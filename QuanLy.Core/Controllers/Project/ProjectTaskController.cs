using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Project
{
    [Route("api/project-task")]
    [ApiController]
    [Description("Quản lý task của dự án")]
    [Authorize]
    public class ProjectTaskController : BaseController<ProjectTasks, ProjectTaskModel, RequestProjectTaskModel, SearchProjectTask>
    {
        private IProjectService projectService;
        private IProjectUserService projectUserService;
        private IProjectTaskService projectTaskService;
        private IHolidayConfigService holidayConfigService;

        public ProjectTaskController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectTasks, ProjectTaskModel, RequestProjectTaskModel, SearchProjectTask>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectTaskService>();
            projectService = serviceProvider.GetRequiredService<IProjectService>();
            holidayConfigService = serviceProvider.GetRequiredService<IHolidayConfigService>();
            projectTaskService = serviceProvider.GetRequiredService<IProjectTaskService>();
            projectUserService = serviceProvider.GetRequiredService<IProjectUserService>();
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<ProjectTaskModel>(item);
                var taskUserInfos = await this.projectUserService.GetAsync(e => !e.Deleted && e.Active && e.ProjectId == item.ProjectId.Value && e.TaskId == itemModel.Id);
                if (taskUserInfos != null && taskUserInfos.Any()) 
                    itemModel.UserIds = taskUserInfos.Where(e => e.UserId.HasValue).Select(e => e.UserId.Value).ToList();
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới task
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        public override async Task<AppDomainResult> AddItem([FromBody] RequestProjectTaskModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            string Message = "";
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());

            var itemUpdate = mapper.Map<ProjectTasks>(itemModel);
            itemUpdate.Created = DateTime.UtcNow.AddHours(7);
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.Active = true;
            //Task ảnh hưởng tiến độ
            var projectInfo = await this.projectService.GetSingleAsync(e => !e.Deleted && e.Active && e.Id == itemUpdate.ProjectId.Value, e => new Projects()
            {
                StartDate = e.StartDate,
                EndDate = e.EndDate
            });

            // Trường hợp task ảnh hưởng đến task khác
            if (itemUpdate.TaskEffect && projectInfo != null)
            {
                var latestTasks = await this.domainService.GetAsync(e => !e.Deleted && e.Active
                && e.ProjectId == itemUpdate.ProjectId
                && e.TaskEffect
                && e.TaskIndex < itemUpdate.TaskIndex
                );
                if (latestTasks != null && latestTasks.Any())
                {
                    var latestTask = latestTasks.OrderByDescending(e => e.TaskIndex).FirstOrDefault();
                    if (latestTask != null && latestTask.EndTime.HasValue)
                        itemUpdate.StartTime = latestTask.EndTime.Value.AddDays(1);
                }
                else itemUpdate.StartTime = projectInfo.StartDate.Value.Date;

                itemUpdate.EndTime = itemUpdate.StartTime.Value.AddDays(itemUpdate.WorkDayEffect);
                // ------------------------- START Tính toán ngày kết thúc của dự án
                DateTime? dateCheck = null;
                // TỔNG SỐ NGÀY LÀM
                int totalDayWork = itemUpdate.WorkDayEffect;
                // TỔNG SỐ NGÀY NGHỈ
                int totalDayOff = 0;
                if (itemUpdate.StartTime.HasValue)
                {
                    // NGÀY KIỂM TRA
                    dateCheck = itemUpdate.StartTime.Value;
                    // NGÀY KẾT THÚC TASK DỰ TÍNH
                    var workDate = itemUpdate.StartTime.Value.AddDays(itemUpdate.WorkDayEffect - 1);
                    while (dateCheck.Value.Date <= workDate.Date)
                    {
                        var holidayConfig = await this.holidayConfigService.GetSingleAsync(e => !e.Deleted && e.Active
                            && e.FromDate.HasValue && e.FromDate.Value.Date <= dateCheck.Value.Date
                            && e.ToDate.HasValue && e.ToDate.Value.Date >= dateCheck.Value.Date
                            );

                        if (holidayConfig != null)
                        {
                            // TỔNG SỐ NGÀY OFF TRONG KÌ NGHỈ ĐƯỢC CẤU HÌNH
                            var totalHolidayConfigOff = (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                            totalDayOff += totalHolidayConfigOff;
                            workDate = workDate.AddDays(totalHolidayConfigOff);
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
                    itemUpdate.EndTime = itemUpdate.StartTime.Value.AddDays(totalDayWork + totalDayOff - 1);

                }
                // ------------------------- END Tính toán ngày kết thúc của dự án


            }
            Message = await this.domainService.GetExistItemMessage(itemUpdate);
            if (string.IsNullOrEmpty(Message))
            {
                success = await this.domainService.CreateAsync(itemUpdate);
                if (success)
                {
                    // Tạo thông báo cho user


                    // PUSH NOTIDESKTOP


                    appDomainResult.Success = success;
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else throw new Exception("Lỗi trong quá trình xử lý");
            }
            else throw new Exception(Message);

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestProjectTaskModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string Message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());

            var itemUpdate = mapper.Map<ProjectTasks>(itemModel);
            itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
            itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.Active = true;
            //Task ảnh hưởng tiến độ
            var projectInfo = await this.projectService.GetSingleAsync(e => !e.Deleted && e.Active && e.Id == itemUpdate.ProjectId.Value, e => new Projects()
            {
                StartDate = e.StartDate,
                EndDate = e.EndDate
            });

            // Trường hợp task ảnh hưởng đến task khác
            if (itemUpdate.TaskEffect && projectInfo != null)
            {
                var latestTasks = await this.domainService.GetAsync(e => !e.Deleted && e.Active
                && e.ProjectId == itemUpdate.ProjectId
                && e.TaskEffect
                && e.Id != itemUpdate.Id
                && e.TaskIndex < itemUpdate.TaskIndex
                );
                if (latestTasks != null && latestTasks.Any())
                {
                    var latestTask = latestTasks.OrderByDescending(e => e.TaskIndex).FirstOrDefault();
                    if (latestTask != null && latestTask.EndTime.HasValue)
                        itemUpdate.StartTime = latestTask.EndTime.Value.AddDays(1);
                }
                else itemUpdate.StartTime = projectInfo.StartDate.Value.Date;

                itemUpdate.EndTime = itemUpdate.StartTime.Value.AddDays(itemUpdate.WorkDayEffect);
                // ------------------------- START Tính toán ngày kết thúc của dự án
                DateTime? dateCheck = null;
                // TỔNG SỐ NGÀY LÀM
                int totalDayWork = itemUpdate.WorkDayEffect;
                // TỔNG SỐ NGÀY NGHỈ
                int totalDayOff = 0;
                if (itemUpdate.StartTime.HasValue)
                {
                    // NGÀY KIỂM TRA
                    dateCheck = itemUpdate.StartTime.Value;
                    // NGÀY KẾT THÚC TASK DỰ TÍNH
                    var workDate = itemUpdate.StartTime.Value.AddDays(itemUpdate.WorkDayEffect - 1);
                    while (dateCheck.Value.Date <= workDate.Date)
                    {
                        var holidayConfig = await this.holidayConfigService.GetSingleAsync(e => !e.Deleted && e.Active
                            && e.FromDate.HasValue && e.FromDate.Value.Date <= dateCheck.Value.Date
                            && e.ToDate.HasValue && e.ToDate.Value.Date >= dateCheck.Value.Date
                            );

                        if (holidayConfig != null)
                        {
                            // TỔNG SỐ NGÀY OFF TRONG KÌ NGHỈ ĐƯỢC CẤU HÌNH
                            var totalHolidayConfigOff = (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                            totalDayOff += totalHolidayConfigOff;
                            workDate = workDate.AddDays(totalHolidayConfigOff);
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
                    itemUpdate.EndTime = itemUpdate.StartTime.Value.AddDays(totalDayWork + totalDayOff - 1);

                }
                // ------------------------- END Tính toán ngày kết thúc của dự án

            }

            Message = await this.domainService.GetExistItemMessage(itemUpdate);
            if (string.IsNullOrEmpty(Message))
            {
                success = await this.domainService.UpdateAsync(itemUpdate);
                if (success)
                {
                    // PUSH NOTIDESKTOP


                    appDomainResult.Success = success;
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else throw new Exception("Lỗi trong quá trình xử lý");
            }
            else throw new Exception(Message);

            return appDomainResult;
        }

        /// <summary>
        /// Xóa task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            var existTask = await this.domainService.GetSingleAsync(e => e.Id == id);
            if (existTask == null) throw new AppException("Không tìm thấy thông tin task");

            bool success = await this.domainService.DeleteAsync(id);
            if (success)
            {
                existTask.Updated = DateTime.UtcNow.AddHours(7);
                existTask.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                // Cập nhật lại những task sau đó
                await this.projectTaskService.UpdateTaskEffect(existTask, true,true);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }


        /// <summary>
        /// Load danh sách task cho 1 user
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-task-user")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetListDataTask([FromQuery] SearchProjectTask searchProjectTask)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var UserId = LoginContext.Instance.CurrentUser.UserId;
                PagedList<ProjectTasks> pagedData = await this.projectTaskService.GetPagedListTaskUser(UserId,searchProjectTask);
                PagedList<ProjectTaskModel> pagedDataModel = mapper.Map<PagedList<ProjectTaskModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật Task của User
        /// </summary>
        /// <returns></returns>
        [HttpGet("update-task-user")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> UpdateTaskUser([FromQuery] int TaskId)
        {
            var Message = "";
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var UserId = LoginContext.Instance.CurrentUser.UserId;
                Message = await this.projectUserService.UpdateStatusTaskOfUser(UserId, TaskId);
                if (string.IsNullOrEmpty(Message))
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    appDomainResult.Success = true;
                }
                else throw new AppException(Message);
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }
    }
}
