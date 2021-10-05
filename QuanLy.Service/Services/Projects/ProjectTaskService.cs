using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectTaskService : DomainService<ProjectTasks, BaseSearch>, IProjectTaskService
    {
        private IUserCoreService userCoreService;
        public ProjectTaskService(IAppUnitOfWork unitOfWork, IMapper mapper, IUserCoreService userCoreService) : base(unitOfWork, mapper)
        {
            this.userCoreService = userCoreService;
        }

        /// <summary>
        ///  Thêm mới task
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ProjectTasks item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy item");
            // Thêm mới task của project
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
                        Type = isCustomer ? (int)CatalogueEnums.ProjectUserType.User : (int)CatalogueEnums.ProjectUserType.Staff
                    };
                    this.unitOfWork.Repository<ProjectUsers>().Create(projectUsers);
                }
            }
            await this.unitOfWork.SaveAsync();
            result = true;

            // Cập nhật lại task khác nếu ảnh hưởng (Effect Tasks)
            await this.UpdateTaskEffect(item);


            return result;
        }

        /// <summary>
        /// Cập nhật task
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ProjectTasks item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy task");

            var existProjectTask = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if(existProjectTask == null) throw new AppException("Không tìm thấy task");
            var currentProjectTaskCreated = existProjectTask.Created;
            var currentProjectTaskCreatedBy = existProjectTask.CreatedBy;
            existProjectTask = mapper.Map<ProjectTasks>(item);
            item.Created = currentProjectTaskCreated;
            item.CreatedBy = currentProjectTaskCreatedBy;
            this.unitOfWork.Repository<ProjectTasks>().Update(existProjectTask);
            await this.unitOfWork.SaveAsync();
            // Thêm mới người làm task
            if (item.UserIds != null && item.UserIds.Any())
            {
                foreach (var userId in item.UserIds)
                {
                    // Kiểm tra user có tồn tại trong project task chưa
                    // TH1: Tồn tại => update
                    var existUserTask = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                        .Where(e => !e.Deleted && e.Active
                        && e.ProjectId == item.ProjectId.Value
                        && e.TaskId == item.Id
                        && e.UserId == userId
                        ).FirstOrDefaultAsync();
                    if(existUserTask != null)
                    {
                        existUserTask.Updated = DateTime.UtcNow.AddHours(7);
                        existUserTask.UpdatedBy = item.UpdatedBy;
                        this.unitOfWork.Repository<ProjectUsers>().Update(existUserTask);
                    }
                    // TH2: Chưa => add new
                    else
                    {
                        // Kiểm tra user id có thuộc nhóm khách hàng không?
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
                            Type = isCustomer ? (int)CatalogueEnums.ProjectUserType.User : (int)CatalogueEnums.ProjectUserType.Staff
                        };
                        this.unitOfWork.Repository<ProjectUsers>().Create(projectUsers);
                    }
                    
                }
            }
            // Xóa all thông tin người làm task nếu không chọn user
            else
            {
                var existUserTasks = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active && e.ProjectId == item.ProjectId.Value
                    && e.TaskId == e.TaskId
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
            result = true;

            // Cập nhật lại task khác nếu ảnh hưởng (Effect Tasks)
            await this.UpdateTaskEffect(item, true);
            return result;
        }

        /// <summary>
        /// Cập nhật task sau task hiện tại
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task UpdateTaskEffect(ProjectTasks item, bool isUpdated = false)
        {
            var previousTasks = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.TaskIndex >= item.TaskIndex && e.ProjectId == item.ProjectId.Value && e.Id != item.Id).ToListAsync();
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
                        startTime = previousTask.StartTime.Value.AddDays(1);
                        endTime = startTime;
                    }
                    if (!startTime.HasValue) continue;
                    if (indexItem != 0)
                        startTime = previousTasks[indexItem - 1].EndTime.Value.AddDays(1);
                    endTime = startTime.Value.AddDays(previousTask.WorkDayEffect);

                    var dateCheck = startTime.Value.Date;
                    int totalDayWork = Convert.ToInt32((endTime.Value - startTime.Value).TotalDays);
                    for (int i = 0; i < totalDayWork; i++)
                    {
                        dateCheck = startTime.Value.Date.AddDays(i);
                        var holidayConfig = await this.unitOfWork.Repository<HolidayConfigs>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active
                            && e.FromDate.HasValue && e.ToDate.HasValue
                            && e.FromDate.Value.Date <= dateCheck.Date && e.ToDate.Value.Date >= dateCheck.Date
                            ).FirstOrDefaultAsync();
                        // Nếu là T7 hoặc CN => + thêm 1 ngày
                        if (dateCheck.Date.DayOfWeek == DayOfWeek.Saturday || dateCheck.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            totalDayWork += 1;
                            continue;
                        }
                        if (holidayConfig == null) continue;
                        i += (holidayConfig.ToDate.Value - holidayConfig.ToDate.Value).Days + 1;
                        totalDayWork += 1;
                    }
                    endTime = startTime.Value.AddDays(totalDayWork - 1);

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
                }
                await this.unitOfWork.SaveAsync();
            }
        }


    }
}
