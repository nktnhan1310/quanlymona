using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.DbContext;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectTaskService : DomainService<ProjectTasks, SearchProjectTask>, IProjectTaskService
    {
        private IUserCoreService userCoreService;
        protected readonly IAppDbContext Context;
        protected readonly INotificationSingle notificationSingle;
        private IProjectToDoList projectToDoList;
        protected readonly IDeviceBrowserService deviceBrowserService;
        protected readonly IConfiguration configuration;
        protected readonly IHubContext<CommentHub> _hubContext;
        public ProjectTaskService(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IUserCoreService userCoreService,
            IAppDbContext context,
            INotificationSingle notificationSingle,
            IHubContext<CommentHub> _hubContext,
            IDeviceBrowserService deviceBrowserService,
            IProjectToDoList projectToDoList) : base(unitOfWork, mapper)
        {
            this.Context = context;
            this.deviceBrowserService = deviceBrowserService;
            this.configuration = configuration;
            this.notificationSingle = notificationSingle;
            this.userCoreService = userCoreService;
            this.projectToDoList = projectToDoList;
            this.notificationSingle = notificationSingle;
            this._hubContext = _hubContext;
        }

        /// <summary>
        ///  Thêm mới task
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ProjectTasks item)
        {
            if (item == null) throw new AppException("Không tìm thấy item");
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            // Thêm mới task của project
            using (var contextTransactionTask = Context.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<ProjectTasks>().Create(item);
                    await this.unitOfWork.SaveAsync();
                    // Thêm mới người làm task
                    if (item.UserIds != null && item.UserIds.Any())
                    {
                        foreach (var userId in item.UserIds)
                        {
                            // Kiểm tra user id có thuộc nhóm khách hàng không?
                            bool isCustomer = await this.userCoreService.IsInUserGroup(userId, Contants.USER_GROUP_CUSTOMER);
                            var userInfo = await this.unitOfWork.Repository<UserCores>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
                            ProjectUsers projectUsers = new ProjectUsers()
                            {
                                Deleted = false,
                                Active = true,
                                Created = DateTime.UtcNow.AddHours(7),
                                CreatedBy = item.CreatedBy,
                                ProjectId = item.ProjectId,
                                TaskId = item.Id,
                                UserId = userId,
                                Type = isCustomer ? (int)CatalogueEnums.ProjectUserType.Staff : (int)CatalogueEnums.ProjectUserType.User
                            };
                            this.unitOfWork.Repository<ProjectUsers>().Create(projectUsers);
                            // Tạo thông báo
                            var DataNotification = await this.notificationSingle.CreateNotification(0
                            , "Bạn nhận được một task mới", $"Admin/Task/TaskList?task-detail/{item.Id.ToString()}"
                            , userId
                            , 2, item.CreatedBy);
                            await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);


                        }
                    }
                    await this.unitOfWork.SaveAsync();

                    // Cập nhật lại task khác nếu ảnh hưởng (Effect Tasks)
                    await this.UpdateTaskEffect(item);
                    await this.projectToDoList.Insert(item.ProjectToDoLists, item.Id, (int)item.ProjectId);

                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    // THÔNG BÁO ONESIGNAL
                    var ListPlayerIds = await this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable()
                        .Where(x => item.UserIds.Contains(x.UserId.Value))
                        .Select(x => x.PlayerId).ToListAsync();
                    if (ListPlayerIds != null && ListPlayerIds.Any())
                    {
                        foreach (var ListPlayerId in ListPlayerIds)
                        {
                            List<string> ListData = new List<string>();
                            ListData.Add(ListPlayerId);
                            string Url = "https://zingnews.vn/";
                            var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", "Bạn nhận được một task mới", ListData, AppId, AppSecret, Url);
                            if (string.IsNullOrEmpty(DataPlayerId))
                            {
                                await this.deviceBrowserService.UpdateHide(ListPlayerId);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;

                }
            }
        }

        /// <summary>
        /// Cập nhật task
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ProjectTasks item)
        {
            List<int> ListUserCreate = new List<int>();
            if (item == null) throw new AppException("Không tìm thấy task");
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            var existProjectTask = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existProjectTask == null) throw new AppException("Không tìm thấy task");
            var currentProjectTaskCreated = existProjectTask.Created;
            var currentProjectTaskCreatedBy = existProjectTask.CreatedBy;
            existProjectTask = mapper.Map<ProjectTasks>(item);
            item.Created = currentProjectTaskCreated;
            item.CreatedBy = currentProjectTaskCreatedBy;

            using (var contextTransactionTask = Context.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<ProjectTasks>().Update(existProjectTask);
                    await this.unitOfWork.SaveAsync();

                    // Cập nhật lại task khác nếu ảnh hưởng (Effect Tasks)
                    await this.UpdateTaskEffect(item, true);
                    await this.projectToDoList.Update(item.ProjectToDoLists, item.Id, (int)item.ProjectId);


                    // Thêm mới người làm task
                    if (item.UserIds != null && item.UserIds.Any())
                    {

                        // Lấy tất cả user co trong task
                        var existUserTasks = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                        .Where(e => !e.Deleted && e.Active && e.ProjectId == item.ProjectId.Value
                        && e.TaskId == item.Id
                        ).ToListAsync();
                        var notExistUserTasks = existUserTasks.Where(x => !item.UserIds.Contains(x.UserId ?? 0)).ToList();
                        if (notExistUserTasks != null && notExistUserTasks.Any())
                        {
                            var todoListUsers = new List<ProjectToDoList>();
                            foreach (var notExistUserTask in notExistUserTasks)
                            {
                                //Xóa User không được chọn vào task
                                this.unitOfWork.Repository<ProjectUsers>().Delete(notExistUserTask);
                                var todoLists = await this.unitOfWork.Repository<ProjectToDoList>().GetQueryable()
                                    .Where(e => e.TaskId == notExistUserTask.TaskId && e.UserId == notExistUserTask.UserId)
                                    .ToListAsync();
                                if (todoLists != null && todoLists.Any())
                                {
                                    todoListUsers.AddRange(todoLists);
                                }
                            }
                            // LẤY RA DANH SÁCH TODOLIST CỦA TASKS
                            //var ToDoListUsers = await this.unitOfWork.Repository<ProjectToDoList>().GetQueryable()
                            //        .Where(e => !e.Deleted && e.Active
                            //        && notExistUserTasks.Any(x => x.TaskId == e.TaskId && x.UserId == e.UserId)
                            //        ).ToListAsync();
                            if (todoListUsers != null && todoListUsers.Any())
                            {
                                foreach (var ToDoListUser in todoListUsers)
                                {
                                    // Xóa tất cả To Do List của User Trong task
                                    this.unitOfWork.Repository<ProjectToDoList>().Delete(ToDoListUser);
                                }
                            }
                        }

                        foreach (var userId in item.UserIds)
                        {
                            var existUserTask = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                                .AnyAsync(e => !e.Deleted && e.Active
                                && e.ProjectId == item.ProjectId.Value
                                && e.TaskId == item.Id
                                && e.UserId == userId);
                            if (existUserTask == false)
                            {
                                ListUserCreate.Add(userId);
                                bool isCustomer = await this.userCoreService.IsInUserGroup(userId, Contants.USER_GROUP_CUSTOMER);
                                ProjectUsers projectUsers = new ProjectUsers()
                                {
                                    Deleted = false,
                                    Active = true,
                                    Created = DateTime.UtcNow.AddHours(7),
                                    CreatedBy = item.CreatedBy,
                                    ProjectId = item.ProjectId,
                                    TaskId = item.Id,
                                    UserId = userId,
                                    Type = isCustomer ? (int)CatalogueEnums.ProjectUserType.Staff : (int)CatalogueEnums.ProjectUserType.User
                                };
                                this.unitOfWork.Repository<ProjectUsers>().Create(projectUsers);
                                var DataNotification = await this.notificationSingle.CreateNotification(0, "Bạn nhận được một task mới", $"Admin/Task/TaskList?task-detail/{item.Id.ToString()}", userId, 2, item.CreatedBy);
                                await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
                            }
                            //// Kiểm tra user có tồn tại trong project task chưa
                            //// TH1: Tồn tại => update
                            //var existUserTask = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                            //    .Where(e => !e.Deleted && e.Active
                            //    && e.ProjectId == item.ProjectId.Value
                            //    && e.TaskId == item.Id
                            //    && e.UserId == userId
                            //    ).FirstOrDefaultAsync();
                            //if (existUserTask != null)
                            //{
                            //    existUserTask.Updated = DateTime.UtcNow.AddHours(7);
                            //    existUserTask.UpdatedBy = item.UpdatedBy;
                            //    this.unitOfWork.Repository<ProjectUsers>().Update(existUserTask);
                            //}
                            //// TH2: Chưa => add new
                            //else
                            //{
                            //    // Kiểm tra user id có thuộc nhóm khách hàng không?
                            //    bool isCustomer = await this.userCoreService.IsInUserGroup(userId, Contants.USER_GROUP_CUSTOMER);
                            //    ProjectUsers projectUsers = new ProjectUsers()
                            //    {
                            //        Deleted = false,
                            //        Active = true,
                            //        Created = DateTime.UtcNow.AddHours(7),
                            //        CreatedBy = item.CreatedBy,
                            //        ProjectId = item.ProjectId,
                            //        TaskId = item.Id,
                            //        UserId = userId,
                            //        Type = isCustomer ? (int)CatalogueEnums.ProjectUserType.User : (int)CatalogueEnums.ProjectUserType.Staff
                            //    };
                            //    this.unitOfWork.Repository<ProjectUsers>().Create(projectUsers);
                            //}
                        }
                    }
                    // Xóa all thông tin người làm task nếu không chọn user
                    else
                    {
                        var existUserTasks = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active && e.ProjectId == item.ProjectId.Value
                            && e.TaskId == item.Id
                            ).ToListAsync();
                        if (existUserTasks != null && existUserTasks.Any())
                        {
                            foreach (var existUserTask in existUserTasks)
                            {
                                this.unitOfWork.Repository<ProjectUsers>().Delete(existUserTask);
                            }
                        }
                    }
                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    // THÔNG BÁO ONESIGNAL
                    var ListPlayerIds = await this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable()
                        .Where(x => ListUserCreate.Contains(x.UserId.Value))
                        .Select(x => x.PlayerId).ToListAsync();
                    if (ListPlayerIds != null && ListPlayerIds.Any())
                    {
                        foreach (var ListPlayerId in ListPlayerIds)
                        {
                            List<string> ListData = new List<string>();
                            ListData.Add(ListPlayerId);
                            string Url = "https://zingnews.vn/";
                            var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", "Bạn nhận được một task mới", ListData, AppId, AppSecret, Url);
                            if (string.IsNullOrEmpty(DataPlayerId))
                            {
                                await this.deviceBrowserService.UpdateHide(ListPlayerId);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Cập nhật task sau task hiện tại
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task UpdateTaskEffect(ProjectTasks item, bool isUpdated = false, bool isDeleted = false)
        {
            var previousTasks = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.TaskIndex >= item.TaskIndex
                && e.ProjectId == item.ProjectId.Value
                && e.Id != item.Id)
                .OrderBy(e => e.TaskIndex)
                .ThenBy(e => e.EndTime)
                .ToListAsync();
            // CẬP NHẬT LẠI THỜI GIAN CHO TẤT CẢ TASK SAU TASK HIỆN TẠI (ProjectTasks item)
            if (previousTasks != null && previousTasks.Any())
            {
                foreach (var previousTask in previousTasks)
                {
                    var indexItem = previousTasks.IndexOf(previousTask);
                    var index = previousTask.TaskIndex;
                    DateTime? startTime = null;
                    DateTime? endTime = null;
                    if (previousTask.StartTime.HasValue)
                    {
                        if (isDeleted)
                        {
                            startTime = item.StartTime.Value;
                            endTime = startTime;
                        }
                        else
                        {
                            startTime = item.EndTime.Value.AddDays(1);
                            endTime = startTime;
                        }
                    }
                    if (!startTime.HasValue) continue;
                    if (indexItem != 0)
                        startTime = previousTasks[indexItem - 1].EndTime.Value.AddDays(1);
                    endTime = startTime.Value.AddDays(previousTask.WorkDayEffect);

                    //var dateCheck = startTime.Value.Date;
                    //int totalDayWork = Convert.ToInt32((endTime.Value - startTime.Value).TotalDays);
                    //for (int i = 0; i < totalDayWork; i++)
                    //{
                    //    dateCheck = startTime.Value.Date.AddDays(i);
                    //    var holidayConfig = await this.unitOfWork.Repository<HolidayConfigs>().GetQueryable()
                    //        .Where(e => !e.Deleted && e.Active
                    //        && e.FromDate.HasValue && e.ToDate.HasValue
                    //        && e.FromDate.Value.Date <= dateCheck.Date && e.ToDate.Value.Date >= dateCheck.Date
                    //        ).FirstOrDefaultAsync();
                    //    // Nếu là T7 hoặc CN => + thêm 1 ngày
                    //    if (dateCheck.Date.DayOfWeek == DayOfWeek.Saturday || dateCheck.Date.DayOfWeek == DayOfWeek.Sunday)
                    //    {
                    //        totalDayWork += 1;
                    //        continue;
                    //    }
                    //    if (holidayConfig == null) continue;
                    //    i += (holidayConfig.ToDate.Value - holidayConfig.ToDate.Value).Days + 1;
                    //    totalDayWork += 1;
                    //}
                    //endTime = startTime.Value.AddDays(totalDayWork - 1);

                    DateTime? dateCheck = null;
                    // TỔNG SỐ NGÀY LÀM
                    int totalDayWork = previousTask.WorkDayEffect;
                    // TỔNG SỐ NGÀY NGHỈ
                    int totalDayOff = 0;
                    // NGÀY KIỂM TRA
                    dateCheck = startTime.Value;
                    // NGÀY KẾT THÚC TASK DỰ TÍNH
                    var workDate = startTime.Value.AddDays(previousTask.WorkDayEffect - 1);
                    while (dateCheck.Value.Date <= workDate.Date)
                    {
                        var holidayConfig = await this.unitOfWork.Repository<HolidayConfigs>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active
                            && e.FromDate.HasValue && e.FromDate.Value.Date <= dateCheck.Value.Date
                            && e.ToDate.HasValue && e.ToDate.Value.Date >= dateCheck.Value.Date
                            ).FirstOrDefaultAsync();

                        if (holidayConfig != null)
                        {
                            // TỔNG SỐ NGÀY OFF TRONG KÌ NGHỈ ĐƯỢC CẤU HÌNH
                            var totalHolidayConfigOff = (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                            totalDayOff += totalHolidayConfigOff;
                            workDate = workDate.AddDays(totalHolidayConfigOff - 1);
                            dateCheck = dateCheck.Value.AddDays(totalHolidayConfigOff);
                            continue;
                        }
                        else
                        {
                            if (dateCheck.Value.Date.DayOfWeek == DayOfWeek.Saturday || dateCheck.Value.Date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                workDate = workDate.AddDays(1);
                                totalDayOff++;
                            }
                        }
                        dateCheck = dateCheck.Value.AddDays(1);
                    }
                    endTime = startTime.Value.AddDays(totalDayWork + totalDayOff - 1);

                    previousTask.StartTime = startTime;
                    previousTask.EndTime = endTime;
                    previousTask.Updated = DateTime.UtcNow.AddHours(7);
                    if (isUpdated) previousTask.UpdatedBy = item.UpdatedBy;
                    else previousTask.UpdatedBy = item.CreatedBy;

                    Expression<Func<ProjectTasks, object>>[] includeProperties = new Expression<Func<ProjectTasks, object>>[]
                    {
                        e => e.StartTime,
                        e => e.EndTime,
                        e => e.Updated,
                        e => e.UpdatedBy,
                    };
                    this.unitOfWork.Repository<ProjectTasks>().UpdateFieldsSave(previousTask, includeProperties);
                    Context.Entry<ProjectTasks>(previousTask).State = EntityState.Detached;
                }
                await this.unitOfWork.SaveAsync();
            }
        }

        public override async Task<string> GetExistItemMessage(ProjectTasks item)
        {
            string message = "";
            if (item.UserIds == null && item.UserIds.Any())
            {
                message = "Phải chọn user phụ trách task";
            }
            else if (item.ProjectToDoLists != null && item.ProjectToDoLists.Any())
            {
                List<int> ListUserToDoList = new List<int>();
                ListUserToDoList = item.ProjectToDoLists.Select(x => x.UserId ?? 0).ToList();
                if (ListUserToDoList.Any(x => !item.UserIds.Contains(x)))
                {
                    message = "User không có trong task";
                }
            }
            return message;
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_ProjectTask_PagingData";
        }
        protected override SqlParameter[] GetSqlParameters(SearchProjectTask baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ProjectId", baseSearch.ProjectId),
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@Status", baseSearch.Status),

            };
            return parameters;
        }
        public override async Task<PagedList<ProjectTasks>> GetPagedListData(SearchProjectTask baseSearch)
        {
            PagedList<ProjectTasks> pagedList = new PagedList<ProjectTasks>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        private async Task<PagedList<ProjectTasks>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<ProjectTasks> pagedList = new PagedList<ProjectTasks>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.Items = MappingDataTable.ConvertToList<ProjectTasks>(dataTable);
                    if (pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }
        public async Task<PagedList<ProjectTasks>> GetPagedListTaskUser(int UserId, SearchProjectTask baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@USERID", UserId),
                new SqlParameter("@ProjectId", baseSearch.ProjectId),
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@Status", baseSearch.Status),
            };
            return await Task.Run(() =>
            {
                PagedList<ProjectTasks> pagedList = new PagedList<ProjectTasks>();
                pagedList.PageIndex = baseSearch.PageIndex;
                pagedList.PageSize = baseSearch.PageSize;
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Mona_sp_Load_ProjectTask_User_By_Id_PagingData";
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.Items = MappingDataTable.ConvertToList<ProjectTasks>(dataTable);
                    if (pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });

        }

        public async Task UpdateStatusTask(int TaskId, int StatusDay)
        {
            var GetData = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().FirstOrDefaultAsync(x => x.Id == TaskId);
            GetData.StatusDay = StatusDay;
            GetData.UpdatedBy = "Server";
            this.unitOfWork.Repository<ProjectTasks>().Update(GetData);
            await this.unitOfWork.SaveAsync();
            Context.Entry<ProjectTasks>(GetData).State = EntityState.Detached;
        }

        private async Task UpdateEndDateTask(int TaskId, DateTime EndTime)
        {
            var GetData = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().FirstOrDefaultAsync(x => x.Id == TaskId);
            GetData.EndTime = EndTime;
            GetData.UpdatedBy = "Server";
            this.unitOfWork.Repository<ProjectTasks>().Update(GetData);
            await this.unitOfWork.SaveAsync();
            Context.Entry<ProjectTasks>(GetData).State = EntityState.Detached;
        }
        public async Task JobProjectTask()
        {
            var ListTasks = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().Where(
                x => !x.IsDone &&
                !x.Deleted &&
                x.Active
                ).ToListAsync();
            foreach (var item in ListTasks)
            {
                if (item.EndTime.Value.Date < DateTime.Now.Date)
                {
                    await this.UpdateStatusTask(item.Id, 3);
                    if (item.TaskInfinity && item.TaskEffect && !item.IsDone)
                    {
                        DateTime startTime, endTime = new DateTime();
                        int daywork = item.WorkDayEffect + 1;
                        var DataProject = await this.unitOfWork.Repository<Projects>().GetQueryable().FirstOrDefaultAsync(e => e.Id == item.ProjectId);
                        var LayTaskTruocDo = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().Where(
                            e => e.Id != item.Id &&
                            e.TaskIndex < item.TaskIndex
                            ).OrderByDescending(x => x.TaskIndex)
                            .ThenByDescending(x => x.EndTime)
                            .FirstOrDefaultAsync();
                        if (LayTaskTruocDo != null)
                            startTime = LayTaskTruocDo.EndTime.Value.AddDays(1);
                        else startTime = DataProject.StartDate.Value.Date;

                        endTime = startTime.AddDays(daywork);
                        DateTime? dateCheck = null;
                        // TỔNG SỐ NGÀY LÀM
                        int totalDayWork = item.WorkDayEffect + 1;
                        // TỔNG SỐ NGÀY NGHỈ
                        int totalDayOff = 0;
                        // NGÀY KIỂM TRA
                        dateCheck = startTime;
                        // NGÀY KẾT THÚC TASK DỰ TÍNH
                        var workDate = startTime.AddDays(item.WorkDayEffect - 1);
                        while (dateCheck.Value.Date <= workDate.Date)
                        {
                            var holidayConfig = await this.unitOfWork.Repository<HolidayConfigs>().GetQueryable()
                                .Where(e => !e.Deleted && e.Active
                                && e.FromDate.HasValue && e.FromDate.Value.Date <= dateCheck.Value.Date
                                && e.ToDate.HasValue && e.ToDate.Value.Date >= dateCheck.Value.Date
                                ).FirstOrDefaultAsync();

                            if (holidayConfig != null)
                            {
                                // TỔNG SỐ NGÀY OFF TRONG KÌ NGHỈ ĐƯỢC CẤU HÌNH
                                var totalHolidayConfigOff = (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                                totalDayOff += totalHolidayConfigOff;
                                workDate = workDate.AddDays(totalHolidayConfigOff - 1);
                                dateCheck = dateCheck.Value.AddDays(totalHolidayConfigOff);
                                continue;
                            }
                            else
                            {
                                if (dateCheck.Value.Date.DayOfWeek == DayOfWeek.Saturday || dateCheck.Value.Date.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    workDate = workDate.AddDays(1);
                                    totalDayOff++;
                                }
                            }
                            dateCheck = dateCheck.Value.AddDays(1);
                        }
                        endTime = startTime.AddDays(totalDayWork + totalDayOff - 1);
                        await this.UpdateEndDateTask(item.Id, endTime);
                        await this.UpdateTaskEffect(item, true);
                    }
                }
            }
        }
        public async Task<string> UpdateTaskInfinity(int TaskId, bool Infinity)
        {
            var message = "";
            var getData = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable()
                .Where(e =>
                e.Id == TaskId
                ).FirstOrDefaultAsync();
            if(message == null)
            {
                message = "Không có dữ liệu task";
            }
            getData.TaskInfinity = Infinity;
            this.unitOfWork.Repository<ProjectTasks>().Update(getData);
            await this.unitOfWork.SaveAsync();
            return message;
        }
    }
}
