using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using QuanLy.Model;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/color-task")]
    [ApiController]
    [Authorize]
    [Description("Quản lý màu của task")]
    public class ColorTaskController : BaseCatalogueController<ColorTasks, ColorTaskModel, RequestColorTaskModel, BaseSearch>
    {
        protected IConfiguration configuration;
        public ColorTaskController(IServiceProvider serviceProvider, ILogger<BaseController<ColorTasks, ColorTaskModel, RequestColorTaskModel, BaseSearch>> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IColorTaskService>();

            this.configuration = configuration;
        }

        /// <summary>
        /// Thêm mới color task
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestColorTaskModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<ColorTasks>(itemModel);
                item.Created = DateTime.UtcNow.AddHours(7);
                item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    string folderUploadPath = string.Empty;
                    string filePath = string.Empty;
                    if (!string.IsNullOrEmpty(item.LinkImg))
                    {
                        // Kiểm tra có tồn tại file trong temp chưa?
                        folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, item.LinkImg);
                        string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.COLOR_TASK_FOLDER);
                        string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.COLOR_TASK_FOLDER, Path.GetFileName(filePath));
                            item.LinkImg = fileUrl;
                        }

                    }
                    success = await this.catalogueService.CreateAsync(item);
                    if (success)
                    {
                        // Xóa file trong folder temp nếu có
                        if (!string.IsNullOrEmpty(folderUploadPath))
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }


                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        // Xóa file trong folder temp nếu có
                        if (!string.IsNullOrEmpty(folderUploadPath))
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                        // Xóa file trong folder upload
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin color task
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestColorTaskModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<ColorTasks>(itemModel);
                item.Updated = DateTime.UtcNow.AddHours(7);
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    string folderUploadPath = string.Empty;
                    string filePath = string.Empty;
                    if (!string.IsNullOrEmpty(item.LinkImg))
                    {
                        // Kiểm tra có tồn tại file trong temp chưa?
                        folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, item.LinkImg);
                        string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.COLOR_TASK_FOLDER);
                        string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.COLOR_TASK_FOLDER, Path.GetFileName(filePath));
                            item.LinkImg = fileUrl;
                        }

                    }
                    success = await this.catalogueService.UpdateAsync(item);
                    if (success)
                    {
                        // Xóa file trong folder temp nếu có
                        if (!string.IsNullOrEmpty(folderUploadPath))
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        // Xóa file trong folder temp nếu có
                        if (!string.IsNullOrEmpty(folderUploadPath))
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                        // Xóa file trong folder upload
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }
    }
}
