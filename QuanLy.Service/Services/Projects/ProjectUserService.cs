using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.DbContext;
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
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectUserService : DomainService<ProjectUsers, BaseSearch>, IProjectUserService
    {
        protected readonly IAppDbContext Context;
        protected readonly IDeviceBrowserService deviceBrowserService;
        protected readonly INotificationSingle notificationSingle;
        protected readonly IHubContext<CommentHub> _hubContext;
        protected readonly IConfiguration configuration;
        public ProjectUserService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IDeviceBrowserService deviceBrowserService
            , INotificationSingle notificationSingle
            , IHubContext<CommentHub> _hubContext
            , IConfiguration configuration
            , IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.deviceBrowserService = deviceBrowserService;
            this.Context = context;
            this.configuration = configuration;
            this.notificationSingle = notificationSingle;
            this._hubContext = _hubContext;
        }

        /// <summary>
        /// Thêm mới thông tin user của dự án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ProjectUsers item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy item");
            bool isExistUser = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable().AnyAsync(e => !e.Deleted && e.Active
            && e.ProjectId == item.ProjectId && e.UserId == item.Id && e.Id != item.Id);
            if (isExistUser) throw new AppException("Thông tin user đã tồn tại trong dự án");
            this.unitOfWork.Repository<ProjectUsers>().Create(item);
            await this.unitOfWork.SaveAsync();
            result = true;
            return result;
        }

        public async Task<string> UpdateStatusTaskOfUser(int UserId, int TaskId)
        {
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            string message = "";
            var GetData = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable().FirstOrDefaultAsync(
                x => x.TaskId == TaskId
                && x.UserId == UserId);
            var GetDatatask = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().FirstOrDefaultAsync(
                x => x.Id == TaskId);
            if (GetData == null)
                message = "Không có dữ liệu của task";
            else if(GetDatatask == null)
                message = "Không có dữ liệu của task";
            else if(GetDatatask.StatusDay == 1)
                message = "Task đã hoàn thành, không được cập nhật nữa";
            else
            {
                GetDatatask.StatusDay = 1;
                GetDatatask.IsDone = true;
                GetDatatask.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                GetDatatask.Updated = DateTime.UtcNow.AddHours(7);
                this.unitOfWork.Repository<ProjectTasks>().Update(GetDatatask);
                await this.unitOfWork.SaveAsync();
                var listUsers = await this.Mona_sp_LoadUser_Role_LeaderAndManager();
                foreach(var item in listUsers)
                {
                    var DataNotification = await this.notificationSingle.CreateNotification(0, "Hoàn thành Task", $"Admin/Task/TaskList?task-detail/{GetData.TaskId.ToString()}", item.Id, 1, Contants.UPDATE_BY_SEVER);
                    await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
                    if (item.dataPlayerIds != null)
                    {
                        foreach (var PlayerIdLeader in item.dataPlayerIds)
                        {
                            List<string> ListData = new List<string>();
                            ListData.Add(PlayerIdLeader.PlayerId);
                            string Url = "https://zingnews.vn/";
                            var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", "Hoàn thành task", ListData, AppId, AppSecret, Url);
                            if (string.IsNullOrEmpty(DataPlayerId))
                            {
                                await this.deviceBrowserService.UpdateHide(PlayerIdLeader.PlayerId);
                            }
                        }
                    }
                }
            }
            return message;
        }

        private async Task<List<Users>> Mona_sp_LoadUser_Role_LeaderAndManager()
        {
            return await Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    List<Users> Model = new List<Users>();
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Mona_sp_LoadUser_Role_LeaderAndManager";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    Model = MappingDataTable.ConvertToList<Users>(dataTable);
                    return Model;
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
    }
}
