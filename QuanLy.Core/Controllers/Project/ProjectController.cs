using App.Core.Controllers;
using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Interface.Services.Configuration;
using App.Core.Service;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Model;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Project
{
    [Route("api/project")]
    [ApiController]
    //[Authorize]
    [Description("Quản lý dự án")]
    public class ProjectController : BaseController<Projects, ProjectModel, RequestProjectModel, SearchProject>
    {
        protected IConfiguration configuration;
        protected IHolidayConfigService holidayConfigService;
        protected IProjectUserService projectUserService;
        protected IUserService userService;
        protected ISMSEmailTemplateCoreService sMSEmailTemplateService;
        protected ISMSConfigurationCoreService sMSConfigurationService;
        protected IProjectTaskService projectTaskService;

        public ProjectController(IServiceProvider serviceProvider, ILogger<BaseController<Projects, ProjectModel, RequestProjectModel, SearchProject>> logger
            , IConfiguration configuration
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectService>();
            holidayConfigService = serviceProvider.GetRequiredService<IHolidayConfigService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            projectUserService = serviceProvider.GetRequiredService<IProjectUserService>();
            sMSEmailTemplateService = serviceProvider.GetRequiredService<ISMSEmailTemplateCoreService>();
            sMSConfigurationService = serviceProvider.GetRequiredService<ISMSConfigurationCoreService>();
            projectTaskService = serviceProvider.GetRequiredService<IProjectTaskService>();

            this.configuration = configuration;
        }


        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestProjectModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<Projects>(itemModel);
            itemUpdate.Created = DateTime.UtcNow.AddHours(7);
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemUpdate.Active = true;
            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();

            // ------------------------- START Tính toán ngày kết thúc của dự án
            DateTime? endDate = null;
            DateTime? startDate = null;
            DateTime? dateCheck = null;
            int totalDayResult = 0;
            if (itemUpdate.StartDate.HasValue && itemUpdate.DateTypeId.HasValue)
            {
                startDate = itemUpdate.StartDate.Value;
                switch (itemUpdate.DateTypeId)
                {
                    case (int)CatalogueEnums.DateProjectType.Date:
                        {
                            dateCheck = itemUpdate.StartDate.Value.AddDays(itemUpdate.QuantityTime ?? 0);
                        }
                        break;
                    case (int)CatalogueEnums.DateProjectType.Month:
                        {
                            dateCheck = itemUpdate.StartDate.Value.AddDays((itemUpdate.QuantityTime ?? 0) * 7);
                        }
                        break;
                    case (int)CatalogueEnums.DateProjectType.Year:
                        {
                            dateCheck = itemUpdate.StartDate.Value.AddMonths(itemUpdate.QuantityTime ?? 0);
                        }
                        break;
                    default:
                        break;
                }
                // Kiểm tra + tính toán ra ngày kết thúc của dự án
                if (startDate.HasValue && dateCheck.HasValue)
                {
                    totalDayResult = Convert.ToInt32((dateCheck.Value - startDate.Value).TotalDays);
                    int totalDayAdd = 0;
                    // Kiểm tra có ngày nghỉ hoặc t7/cn thì + thêm 1 ngày
                    for (int i = 0; i < totalDayResult; i++)
                    {
                        dateCheck = startDate.Value.Date.AddDays(i);
                        var holidayConfig = await this.holidayConfigService.GetSingleAsync(e => !e.Deleted && e.Active
                        && e.FromDate.HasValue && e.FromDate.Value.Date <= dateCheck.Value.Date
                        && e.ToDate.HasValue && e.ToDate.Value.Date >= dateCheck.Value.Date
                        );
                        if (holidayConfig != null)
                        {
                            totalDayAdd += (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                            i += (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                        }
                        else
                        {
                            if (dateCheck.Value.Date.DayOfWeek == DayOfWeek.Saturday || dateCheck.Value.Date.DayOfWeek == DayOfWeek.Sunday)
                                totalDayAdd++;
                        }
                    }
                    endDate = startDate.Value.AddDays(totalDayAdd);
                    //while (startDate.Value <= dateCheck.Value)
                    //{
                    //    var hospitalConfig = await this.holidayConfigService.GetSingleAsync(e => !e.Deleted && e.Active
                    //    && e.FromDate.HasValue && e.FromDate.Value.Date <= startDate.Value.Date
                    //    && e.ToDate.HasValue && e.ToDate.Value.Date >= startDate.Value.Date
                    //    );

                    //    // T7 hoặc CN => + thêm 1 ngày
                    //    if (startDate.Value.Date.DayOfWeek == DayOfWeek.Saturday || startDate.Value.Date.DayOfWeek == DayOfWeek.Sunday)
                    //    {
                    //        totalDayResult += 1;
                    //        startDate.Value.AddDays(1);
                    //        continue;
                    //    }
                    //    startDate.Value.AddDays(1);
                    //    // Có ngày nghỉ => + thêm 1 ngày
                    //    if (hospitalConfig == null) continue;
                    //    totalDayResult += 1;
                    //}
                    //endDate = startDate.Value.AddDays(totalDayResult);
                }

            }
            // ------------------------- END Tính toán ngày kết thúc của dự án

            // Thông tin file local của project
            if (itemUpdate.ProjectFiles != null && itemUpdate.ProjectFiles.Any())
            {
                foreach (var file in itemUpdate.ProjectFiles)
                {
                    string folderUploadPath = string.Empty;
                    var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                    if (isProduct)
                        folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                    else
                        folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                    string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                    string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER);
                    string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER, Path.GetFileName(filePath));
                    // Kiểm tra có tồn tại file trong temp chưa?
                    if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                    {
                        FileUtils.CreateDirectory(folderUploadUrl);
                        FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                        folderUploadPaths.Add(fileUploadPath);
                        string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER, Path.GetFileName(filePath));
                        filePaths.Add(filePath);
                        file.Created = DateTime.UtcNow.AddHours(7);
                        file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                        file.Active = true;
                        file.Deleted = false;
                        file.FileName = Path.GetFileName(filePath);
                        file.FileExtension = Path.GetExtension(filePath);
                        file.FileUrl = fileUrl;
                    }
                }
                //----------------------- XỬ LÝ FILE TRÊN DRIVE
                //.............................................
                //.............................................

            }
            success = await this.domainService.CreateAsync(itemUpdate);

            if (success)
            {
                // Kiểm tra thông tin khách
                if (!string.IsNullOrEmpty(itemModel.CustomerPhone))
                {
                    var existCustomer = await this.userService.GetSingleAsync(e => !e.Deleted && e.Active && e.Phone == itemModel.CustomerPhone);
                    int? userId = null;
                    // Nếu chưa có thông tin => Thêm mới thông tin khách
                    if (existCustomer == null)
                    {
                        string randomPassword = RandomUtilities.RandomString(8);
                        string password = SecurityUtils.HashSHA1(randomPassword);
                        // Thêm thông tin cho khách
                        Users userCores = new Users()
                        {
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                            Phone = itemModel.CustomerPhone,
                            Email = itemModel.CustomerEmail,
                            UserName = itemModel.CustomerPhone,
                            Password = password
                        };
                        bool successUser = await this.userService.CreateAsync(userCores);
                        if (successUser)
                        {
                            userId = userCores.Id;
                            // Thành công => GỬi SMS cho user + password
                            await this.SendUserPasswordSMS(userCores.Phone, randomPassword);
                        }

                    }
                    else userId = existCustomer.Id;
                    // Thêm user vào thông tin dự án
                    ProjectUsers projectUsers = new ProjectUsers()
                    {
                        Created = DateTime.UtcNow.AddHours(7),
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        ProjectId = itemUpdate.Id,
                        UserId = userId,
                        Type = (int)CatalogueEnums.ProjectUserType.User
                    };
                    await this.projectUserService.CreateAsync(projectUsers);
                }
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            else
            {
                if (folderUploadPaths.Any())
                {
                    foreach (var folderUploadPath in folderUploadPaths)
                    {
                        System.IO.File.Delete(folderUploadPath);
                    }
                }
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                throw new Exception("Lỗi trong quá trình xử lý");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestProjectModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<Projects>(itemModel);
            itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
            itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();
            if (itemUpdate.ProjectFiles != null && itemUpdate.ProjectFiles.Any())
            {
                foreach (var file in itemUpdate.ProjectFiles)
                {
                    // ------- START GET URL FOR FILE
                    string folderUploadPath = string.Empty;
                    var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                    if (isProduct)
                        folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                    else
                        folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                    string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                    string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER);
                    string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER, Path.GetFileName(filePath));
                    // Kiểm tra có tồn tại file trong temp chưa?
                    if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                    {
                        FileUtils.CreateDirectory(folderUploadUrl);
                        FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                        folderUploadPaths.Add(fileUploadPath);
                        string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.PROJECT_FOLDER, Path.GetFileName(filePath));
                        // ------- END GET URL FOR FILE
                        filePaths.Add(filePath);
                        file.Created = DateTime.UtcNow.AddHours(7);
                        file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                        file.Active = true;
                        file.Deleted = false;
                        file.FileName = Path.GetFileName(filePath);
                        file.FileExtension = Path.GetExtension(filePath);
                        file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                        file.FileUrl = fileUrl;
                        file.ProjectId = itemUpdate.Id;
                    }
                    else
                    {
                        file.Updated = DateTime.UtcNow.AddHours(7);
                        file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    }
                }
            }

            success = await this.domainService.UpdateAsync(itemUpdate);
            if (success)
            {
                // Cập nhật task ảnh hưởng
                var taskEffectInfos = await this.projectTaskService.GetAsync(e => !e.Deleted && e.Active && e.TaskEffect);
                if (taskEffectInfos != null && taskEffectInfos.Any())
                {
                    var taskEffect = taskEffectInfos.OrderBy(e => e.TaskIndex).ThenByDescending(e => e.EndTime).FirstOrDefault();
                    if (taskEffect != null && taskEffect.StartTime.HasValue)
                    {
                        DateTime startTime = taskEffect.StartTime.Value.Date;
                        DateTime endTime = startTime.AddDays(taskEffect.WorkDayEffect);

                        DateTime dateCheck = startTime.Date;
                        int totalDayresult = taskEffect.WorkDayEffect;
                        for (int j = 0; j < totalDayresult; j++)
                        {
                            dateCheck = startTime.AddDays(j);
                            var holidayConfig = await this.holidayConfigService.GetSingleAsync(e => !e.Deleted && e.Active
                            && e.FromDate.HasValue && e.ToDate.HasValue
                            && e.FromDate.Value.Date <= dateCheck.Date && e.ToDate.Value.Date >= dateCheck.Date
                            );
                            if (holidayConfig != null)
                            {
                                totalDayresult += (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                                j += (holidayConfig.ToDate.Value - holidayConfig.FromDate.Value).Days + 1;
                            }
                            else
                            {
                                if ((int)dateCheck.DayOfWeek == 6 || (int)dateCheck.DayOfWeek == 0)
                                    totalDayresult++;
                            }
                        }
                        endTime = startTime.AddDays(totalDayresult - 1);

                        taskEffect.StartTime = startTime;
                        taskEffect.EndTime = endTime;
                        taskEffect.Updated = DateTime.UtcNow.AddHours(7);
                        taskEffect.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                        Expression<Func<ProjectTasks, object>>[] includeProperties = new Expression<Func<ProjectTasks, object>>[]
                        {
                            e => e.StartTime,
                            e => e.EndTime,
                            e => e.Updated,
                            e => e.UpdatedBy
                        };
                        await this.projectTaskService.UpdateFieldAsync(taskEffect, includeProperties);
                        // Cập nhật lại task sau đó
                        await this.projectTaskService.UpdateTaskEffect(taskEffect, true);
                    }
                }

                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            else
            {
                if (folderUploadPaths.Any())
                {
                    foreach (var folderUploadPath in folderUploadPaths)
                    {
                        System.IO.File.Delete(folderUploadPath);
                    }
                }
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                throw new Exception("Lỗi trong quá trình xử lý");
            }
            appDomainResult.Success = success;


            return appDomainResult;
        }

        #region Private Methods

        /// <summary>
        /// Gửi SMS thông tin password cho user 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task SendUserPasswordSMS(string phone, string password)
        {
            string content = string.Empty;
            var smsSendPasswordTemplate = await this.sMSEmailTemplateService.GetSingleAsync(e => e.Code == Contants.SMS_PASSWORD_TEMPLATE);
            if (smsSendPasswordTemplate != null) content = string.Format(smsSendPasswordTemplate.Body, phone, password);
            if (!string.IsNullOrEmpty(content))
                await sMSConfigurationService.SendSMS(phone, content);
        }

        #endregion

    }
}
