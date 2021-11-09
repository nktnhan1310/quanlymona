using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities.Newfeed;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Interface.Services.Newfeed;
using QuanLy.Model.Newfeed;
using QuanLy.Model.RequestModel.Newfeed;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Newfeed
{
    [Route("api/post-content")]
    [ApiController]
    [Authorize]
    [Description("Quản lý newfeed ")]
    public class PostContentController : BaseController<PostContents, PostContentModel, RequestPostContentModel, BaseSearch>
    {
        protected IConfiguration configuration;
        protected IUserService userService;
        protected INotificationSingle notificationSingle;
        protected IUserInGroupService userInGroupService;
        public PostContentController(IServiceProvider serviceProvider, ILogger<BaseController<PostContents, PostContentModel, RequestPostContentModel, BaseSearch>> logger
            , IConfiguration configuration
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPostContents>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
            this.configuration = configuration;
            this.notificationSingle = serviceProvider.GetRequiredService<INotificationSingle>();
        }

        /// <summary>
        /// Thêm mới newfeed
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestPostContentModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<PostContents>(itemModel);
            item.Created = DateTime.UtcNow.AddHours(7);
            item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            item.Active = true;
            var FullName = LoginContext.Instance.CurrentUser.FullName;
            var UserId = LoginContext.Instance.CurrentUser.UserId;
            if (!string.IsNullOrEmpty(item.PostIMG))
            {
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                string folderUploadPath = string.Empty;
                var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                if (isProduct)
                    folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                else
                    folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, item.PostIMG);
                string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER);
                string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER, Path.GetFileName(filePath));

                // Kiểm tra có tồn tại file trong temp chưa?
                if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                {
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));

                    // Gắn link vào ảnh 
                    string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER, Path.GetFileName(item.PostIMG));
                    item.PostIMG = fileUrl;
                    folderUploadPaths.Add(fileUploadPath);
                    filePaths.Add(filePath);
                }
                success = await this.domainService.CreateAsync(item);
                if (success)
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    // Remove file trong thư mục temp
                    if (filePaths.Any())
                    {
                        foreach (var paths in filePaths)
                        {
                            System.IO.File.Delete(paths);
                        }
                    }

                    var CheckRoleAdmin = await this.userInGroupService.GetUserInGroupByUserId(UserId);
                    //Role là admin thì tạo thông báo cho tất cả leader 
                    if( CheckRoleAdmin != null && CheckRoleAdmin.UserGroupId == 1)
                    {
                        var DataLink = "Admin/NewFeed/NewFeed/" + item.Id + "";
                        await this.notificationSingle.CreateNotifacationForLeader(item.CreatedBy, FullName, DataLink, "Bài đăng mới từ ");
                    }
                }
                else
                {
                    if (folderUploadPaths.Any())
                    {
                        foreach (var UploadPath in folderUploadPaths)
                        {
                            System.IO.File.Delete(UploadPath);
                        }
                    }
                    // Remove file trong thư mục temp
                    if (filePaths.Any())
                    {
                        foreach (var paths in filePaths)
                        {
                            System.IO.File.Delete(paths);
                        }
                    }
                    throw new Exception("Lỗi trong quá trình xử lý");
                }
            }
            else
            {
                success = await this.domainService.CreateAsync(item);
                if (success)
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    var CheckRoleAdmin = await this.userInGroupService.GetUserInGroupByUserId(UserId);
                    //Role là admin thì tạo thông báo cho tất cả leader 
                    if (CheckRoleAdmin != null && CheckRoleAdmin.UserGroupId == 1)
                    {
                        var DataLink = "Admin/NewFeed/NewFeed/" + item.Id + "";
                        await this.notificationSingle.CreateNotifacationForLeader(item.CreatedBy, FullName, DataLink, "Bài đăng mới từ ");
                    }
                }
                
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin NewFeed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]

        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestPostContentModel itemModel)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<PostContents>(itemModel);
            var DataExist = this.domainService.GetById(item.Id);
            if (DataExist != null)
            {
                var userPresent = LoginContext.Instance.CurrentUser.UserName;
                var userCreateFeed = DataExist.CreatedBy;
                if (userCreateFeed == userPresent)
                {
                    string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER, Path.GetFileName(item.PostIMG));
                    item.UpdatedBy = userPresent;
                    item.Updated = DateTime.UtcNow.AddHours(7);
                    if (fileUrl != DataExist.PostIMG && !string.IsNullOrEmpty(item.PostIMG))
                    {
                        List<string> filePaths = new List<string>();
                        List<string> folderUploadPaths = new List<string>();
                        List<string> filePathsOld = new List<string>();
                        string folderUploadPath = string.Empty;
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                        string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, item.PostIMG);
                        string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER);
                        string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.NEWFEED_FOLDER, Path.GetFileName(filePath));

                        // Xóa ảnh củ
                        if (!string.IsNullOrEmpty(DataExist.PostIMG))
                        {
                            if(System.IO.File.Exists(DataExist.PostIMG))
                            {
                                filePathsOld.Add(DataExist.PostIMG);
                            }
                        }

                        // Kiểm tra có tồn tại file trong temp chưa?
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            item.PostIMG = fileUrl;
                            folderUploadPaths.Add(fileUploadPath);
                            filePaths.Add(filePath);
                        }
                        success = await this.domainService.UpdateAsync(item);
                        if (success)
                        {
                            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                            if (filePathsOld.Any())
                            {
                                // Remove file trong thư mục chính thức xóa ảnh củ
                                foreach (var UploadPath in filePathsOld)
                                {
                                    System.IO.File.Delete(UploadPath);
                                }
                            }
                            // Remove file trong thư mục temp
                            if (filePaths.Any())
                            {
                                foreach (var paths in filePaths)
                                {
                                    System.IO.File.Delete(paths);
                                }
                            }
                        }
                        else
                        {
                            if (folderUploadPaths.Any())
                            {
                                foreach (var UploadPath in folderUploadPaths)
                                {
                                    System.IO.File.Delete(UploadPath);
                                }
                            }
                            // Remove file trong thư mục temp
                            if (filePaths.Any())
                            {
                                foreach (var paths in filePaths)
                                {
                                    System.IO.File.Delete(paths);
                                }
                            }
                            throw new Exception("Lỗi trong quá trình xử lý");
                        }
                    }
                    else
                    {
                        item.PostIMG = DataExist.PostIMG;
                        success = await this.domainService.UpdateAsync(item);
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Không có quyền sửa của User khác");
                }
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            var DataExist = this.domainService.GetById(id);
            if (DataExist != null)
            {
                var userPresent = LoginContext.Instance.CurrentUser.UserName;
                var userCreateFeed = DataExist.CreatedBy;
                if (userCreateFeed == userPresent)
                {
                    success = await this.domainService.DeleteAsync(id);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }

                }
                else
                {
                    throw new KeyNotFoundException("Không có quyền xóa của User khác");
                }
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }
    }
}
