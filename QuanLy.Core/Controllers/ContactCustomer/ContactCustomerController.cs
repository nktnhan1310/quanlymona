using App.Core.Controllers;
using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Configuration;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Entities.Search;
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

namespace QuanLy.Core.Controllers
{
    [Route("api/contact-customer")]
    [ApiController]
    [Description("Quản lý thông tin liên hệ khách hàng")]
    public class ContactCustomerController : BaseController<ContactCustomers, ContactCustomerModel, RequestContactCustomerModel, SearchContactCustomer>
    {
        protected IContactCustomerService contactCustomerService;
        protected IUserService userService;
        protected IConfiguration configuration;
        protected IUserGroupService userGroupService;
        protected ISMSConfigurationCoreService sMSConfigurationService;
        protected ISMSEmailTemplateCoreService sMSEmailTemplateService;
        protected IUserCoreService userCoreService;
        protected IUserGroupCoreService userGroupCoreService;
        protected IContactCustomerMappingRequestService contactCustomerMappingRequestService;
        protected ICompanyService companyService;
        protected IContactCustomerServiceTypeService contactCustomerServiceTypeService;
        protected IContactCustomerNoteService contactCustomerNoteService;
        protected IContactCustomerFileService contactCustomerFileService;
        protected IContactCustomerSaleRequestService contactCustomerSaleRequestService;
        protected IRequestTypeService requestTypeService;
        protected ISourceTypeService sourceTypeService;
        protected IReportSourceService reportSourceService;

        public ContactCustomerController(IServiceProvider serviceProvider, ILogger<BaseController<ContactCustomers, ContactCustomerModel, RequestContactCustomerModel, SearchContactCustomer>> logger
            , IConfiguration configuration
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.configuration = configuration;
            domainService = serviceProvider.GetRequiredService<IContactCustomerService>();
            contactCustomerService = serviceProvider.GetRequiredService<IContactCustomerService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            sMSConfigurationService = serviceProvider.GetRequiredService<ISMSConfigurationCoreService>();
            sMSEmailTemplateService = serviceProvider.GetRequiredService<ISMSEmailTemplateCoreService>();
            companyService = serviceProvider.GetRequiredService<ICompanyService>();
            userCoreService = serviceProvider.GetRequiredService<IUserCoreService>();
            contactCustomerMappingRequestService = serviceProvider.GetRequiredService<IContactCustomerMappingRequestService>();
            contactCustomerServiceTypeService = serviceProvider.GetRequiredService<IContactCustomerServiceTypeService>();
            contactCustomerNoteService = serviceProvider.GetRequiredService<IContactCustomerNoteService>();
            contactCustomerFileService = serviceProvider.GetRequiredService<IContactCustomerFileService>();
            contactCustomerSaleRequestService = serviceProvider.GetRequiredService<IContactCustomerSaleRequestService>();
            requestTypeService = serviceProvider.GetRequiredService<IRequestTypeService>();
            sourceTypeService = serviceProvider.GetRequiredService<ISourceTypeService>();
            reportSourceService = serviceProvider.GetRequiredService<IReportSourceService>();
            userGroupCoreService = serviceProvider.GetRequiredService<IUserGroupCoreService>();

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
                // Mapping yêu cầu
                item.ContactCustomerMappingRequests = await contactCustomerMappingRequestService.GetAsync(e => !e.Deleted && e.Active && e.ContactCustomerId == id);

                if (item.SaleId.HasValue && item.SaleId.Value > 0)
                {
                    var saleInfo = await this.userCoreService.GetSingleAsync(e => !e.Deleted && e.Active && e.Id == item.SaleId.Value);
                    if (saleInfo != null) item.SaleName = saleInfo.UserFullName;
                }

                if (item.ContactCustomerMappingRequests != null && item.ContactCustomerMappingRequests.Any())
                {
                    foreach (var contactCustomerMappingRequest in item.ContactCustomerMappingRequests)
                    {
                        var sourceInfo = await this.sourceTypeService.GetSingleAsync(e => e.Id == contactCustomerMappingRequest.SourceId);
                        item.SourceName = sourceInfo != null ? sourceInfo.Name : string.Empty;
                        var requestInfo = await this.requestTypeService.GetSingleAsync(e => e.Id == contactCustomerMappingRequest.RequestId);
                        item.RequestName = requestInfo != null ? requestInfo.Name : string.Empty;
                    }
                }
                // Dịch vụ của liên hệ
                item.ContactCustomerServices = await contactCustomerServiceTypeService.GetAsync(e => !e.Deleted && e.Active && e.ContactCustomerId == id);

                // Ghi chú của liên hệ
                item.ContactCustomerNotes = await contactCustomerNoteService.GetAsync(e => !e.Deleted && e.Active && e.ContactCustomerId == id);

                // Thông tin file liên hệ
                item.ContactCustomerFiles = await contactCustomerFileService.GetAsync(e => !e.Deleted && e.Active && e.ContactCustomerId == id);

                // Thông tin liên hệ thêm của dịch vụ
                item.ContactCustomerSaleRequests = await contactCustomerSaleRequestService.GetAsync(e => !e.Deleted && e.Active && e.ContactCustomerId == id);

                var itemModel = mapper.Map<ContactCustomerModel>(item);
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
        /// Thêm mới thông tin liên hệ
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestContactCustomerModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<ContactCustomers>(itemModel);
                // Thêm mới thông tin công ty nếu chưa có trong dữ liệu
                if (!string.IsNullOrEmpty(itemModel.CompanyName) && !itemModel.CompanyId.HasValue)
                {
                    var existCompaynyInfo = await this.companyService.GetSingleAsync(e => !e.Deleted && e.Active
                    && (e.Name.ToLower().Contains(itemModel.CompanyName.ToLower())
                    || e.Code.ToLower().Contains(itemModel.CompanyName.ToLower())
                    ));
                    if (existCompaynyInfo == null)
                    {
                        Companies companies = new Companies()
                        {
                            Deleted = false,
                            Active = true,
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                            Code = RandomUtilities.RandomString(8),
                            Name = itemModel.CompanyName,
                        };
                        await this.companyService.CreateAsync(companies);
                        item.CompanyId = companies.Id;
                    }
                    else item.CompanyId = existCompaynyInfo.Id;
                }

                item.Created = DateTime.UtcNow.AddHours(7);
                item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    if (item.ContactCustomerFiles != null && item.ContactCustomerFiles.Any())
                    {
                        foreach (var file in item.ContactCustomerFiles)
                        {
                            // Kiểm tra có tồn tại file trong temp chưa?
                            string folderUploadPath = string.Empty;
                            folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.CONTACT_CUSTOMER_FOLDER);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));

                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.CONTACT_CUSTOMER_FOLDER, Path.GetFileName(filePath));
                                filePaths.Add(filePath);

                                file.Created = DateTime.UtcNow.AddHours(7);
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileUrl = fileUrl;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                            }
                        }
                    }
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                    {
                        // Check thêm thông tin user khách hàng nếu trạng thái CSKH là đã liên hệ
                        if (item.Status == (int)CatalogueEnums.ContactCustomerStatus.Connected)
                        {
                            var existUser = await this.userCoreService.GetSingleAsync(e => !e.Deleted && e.Active && e.Phone == item.Phone);
                            // Không có thông tin user => Thêm mới user
                            if (existUser == null)
                            {
                                // Add nhóm khách cho user
                                var customerGroup = await this.userGroupCoreService.GetSingleAsync(e => e.Code == Contants.USER_GROUP_CUSTOMER);

                                // Generate Password
                                string randomPassword = RandomUtilities.RandomString(8);
                                string password = SecurityUtils.HashSHA1(randomPassword);
                                UserCores userCores = new UserCores()
                                {
                                    Deleted = false,
                                    Active = true,
                                    Created = DateTime.UtcNow.AddHours(7),
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Password = password,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    UserName = item.Phone,
                                    Address = item.Address,
                                    BirthDate = item.BirthDate,
                                    Gender = item.Gender,
                                    UserFullName = item.FullName,
                                    UserGroupIds = customerGroup != null ? new List<int>() { customerGroup.Id } : null,
                                };
                                bool successAddUser = await this.userCoreService.CreateAsync(userCores);
                                // Tạo user thành công => gửi mật khẩu đến user qua sms
                                if (successAddUser)
                                    await this.SendUserPasswordSMS(userCores.Phone, randomPassword);
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
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        // Xóa file trong thư mục upload
                        if (folderUploadPaths.Any())
                        {
                            foreach (var folderUploadPath in folderUploadPaths)
                            {
                                System.IO.File.Delete(folderUploadPath);
                            }
                        }
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin liên hệ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestContactCustomerModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<ContactCustomers>(itemModel);
                // Thêm mới thông tin công ty nếu chưa có trong dữ liệu
                if (!string.IsNullOrEmpty(itemModel.CompanyName) && !itemModel.CompanyId.HasValue)
                {
                    var existCompaynyInfo = await this.companyService.GetSingleAsync(e => !e.Deleted && e.Active
                    && (e.Name.ToLower().Contains(itemModel.CompanyName.ToLower())
                    || e.Code.ToLower().Contains(itemModel.CompanyName.ToLower())
                    ));
                    if (existCompaynyInfo == null)
                    {
                        Companies companies = new Companies()
                        {
                            Deleted = false,
                            Active = true,
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                            Code = RandomUtilities.RandomString(8),
                            Name = itemModel.CompanyName,
                        };
                        await this.companyService.CreateAsync(companies);
                        item.CompanyId = companies.Id;
                    }
                    else item.CompanyId = existCompaynyInfo.Id;
                }
                item.Updated = DateTime.UtcNow.AddHours(7);
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;

                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    if (item.ContactCustomerFiles != null && item.ContactCustomerFiles.Any())
                    {
                        foreach (var file in item.ContactCustomerFiles)
                        {
                            // Kiểm tra có tồn tại file trong temp chưa?
                            string folderUploadPath = string.Empty;
                            folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, Contants.CONTACT_CUSTOMER_FOLDER);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));

                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Contants.CONTACT_CUSTOMER_FOLDER, Path.GetFileName(filePath));
                                filePaths.Add(filePath);

                                file.Created = DateTime.UtcNow.AddHours(7);
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileUrl = fileUrl;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                            }
                            else
                            {
                                file.Updated = DateTime.UtcNow.AddHours(7);
                                file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                            }
                        }
                    }
                    success = await this.domainService.UpdateAsync(item);
                    if (success)
                    {
                        // Check thêm thông tin user khách hàng nếu trạng thái CSKH là đã liên hệ
                        if (item.Status == (int)CatalogueEnums.ContactCustomerStatus.Connected)
                        {
                            var existUser = await this.userCoreService.GetSingleAsync(e => !e.Deleted && e.Active && e.Phone == item.Phone);
                            // Không có thông tin user => Thêm mới user
                            if (existUser == null)
                            {
                                // Add nhóm khách cho user
                                var customerGroup = await this.userGroupCoreService.GetSingleAsync(e => e.Code == Contants.USER_GROUP_CUSTOMER);

                                // Generate Password
                                string randomPassword = RandomUtilities.RandomString(8);
                                string password = SecurityUtils.HashSHA1(randomPassword);
                                UserCores userCores = new UserCores()
                                {
                                    Deleted = false,
                                    Active = true,
                                    Created = DateTime.UtcNow.AddHours(7),
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Password = password,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    UserName = item.Phone,
                                    Address = item.Address,
                                    BirthDate = item.BirthDate,
                                    Gender = item.Gender,
                                    UserFullName = item.FullName,
                                    UserGroupIds = customerGroup != null ? new List<int>() { customerGroup.Id } : null,
                                };
                                bool successAddUser = await this.userCoreService.CreateAsync(userCores);
                                // Tạo user thành công => gửi mật khẩu đến user qua sms
                                if (successAddUser)
                                    await this.SendUserPasswordSMS(userCores.Phone, randomPassword);
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
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        // Xóa file trong thư mục upload
                        if (folderUploadPaths.Any())
                        {
                            foreach (var folderUploadPath in folderUploadPaths)
                            {
                                System.IO.File.Delete(folderUploadPath);
                            }
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
        /// Merge thông tin liên hệ
        /// </summary>
        /// <returns></returns>
        [HttpPost("merge-contact-customer")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> MergeContactCustomer([FromBody] RequestMergeContactCustomerModel requestMergeContactCustomerModel)
        {
            if (requestMergeContactCustomerModel != null && requestMergeContactCustomerModel.ContactCustomerIds != null && requestMergeContactCustomerModel.ContactCustomerIds.Any())
            {
                await this.contactCustomerService.MergeContactCustomers(requestMergeContactCustomerModel.ContactCustomerIds, LoginContext.Instance.CurrentUser.UserName);
            }
            return new AppDomainResult
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật nhanh trạng thái chăm sóc khách hàng
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost("update-cskh-status")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateCSKHStatus([FromBody] int contactCustomerId, int status)
        {
            if (contactCustomerId <= 0)
                throw new AppException("Mã liên hệ khách hàng không hợp lệ");
            var existContactCustomer = await this.contactCustomerService.GetByIdAsync(contactCustomerId);
            if (existContactCustomer != null)
            {
                existContactCustomer.Status = status;
                existContactCustomer.Updated = DateTime.UtcNow.AddHours(7);
                existContactCustomer.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                {
                    e => e.Status,
                    e => e.Updated,
                    e => e.UpdatedBy
                };
                await this.contactCustomerService.UpdateFieldAsync(existContactCustomer, includeProperties);

                // Thêm mới thông tin khách hàng nếu trạng thái là đã liên hệ
                if (status == (int)CatalogueEnums.ContactCustomerStatus.Connected)
                {
                    var existUser = await this.userCoreService.GetSingleAsync(e => !e.Deleted && e.Active && e.Phone == existContactCustomer.Phone);
                    // Không có thông tin user => Thêm mới user
                    if (existUser == null)
                    {
                        // Add nhóm khách cho user
                        var customerGroup = await this.userGroupCoreService.GetSingleAsync(e => e.Code == Contants.USER_GROUP_CUSTOMER);

                        // Generate Password
                        string randomPassword = RandomUtilities.RandomString(8);
                        string password = SecurityUtils.HashSHA1(randomPassword);
                        UserCores userCores = new UserCores()
                        {
                            Deleted = false,
                            Active = true,
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                            Password = password,
                            Email = existContactCustomer.Email,
                            Phone = existContactCustomer.Phone,
                            UserName = existContactCustomer.Phone,
                            Address = existContactCustomer.Address,
                            BirthDate = existContactCustomer.BirthDate,
                            Gender = existContactCustomer.Gender,
                            UserFullName = existContactCustomer.FullName,
                            UserGroupIds = customerGroup != null ? new List<int>() { customerGroup.Id } : null,
                        };
                        bool successAddUser = await this.userCoreService.CreateAsync(userCores);
                        // Tạo user thành công => gửi mật khẩu đến user qua sms
                        if (successAddUser)
                            await this.SendUserPasswordSMS(userCores.Phone, randomPassword);
                    }
                }
            }
            else throw new AppException("Không tìm thấy thông tin liên hệ");

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật trạng thái sale
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="saleStatus"></param>
        /// <returns></returns>
        [HttpPost("update-sale-status")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateSaleStatus([FromBody] int contactCustomerId, int saleStatus)
        {
            if (contactCustomerId <= 0)
                throw new AppException("Mã liên hệ khách hàng không hợp lệ");
            bool isLeader = await this.userCoreService.IsInUserGroup(LoginContext.Instance.CurrentUser.UserId, Contants.USER_GROUP_LEADER);
            var existContactCustomer = await this.contactCustomerService.GetByIdAsync(contactCustomerId);
            // CHECK QUYỀN LEADER HOẶC SALER THÌ MỚI ĐƯỢC CẬP NHẬT TRẠNG THÁI SALE
            if (isLeader || existContactCustomer.SaleId == LoginContext.Instance.CurrentUser.UserId)
            {
                if (existContactCustomer != null)
                {
                    existContactCustomer.SaleStatus = saleStatus;
                    existContactCustomer.Updated = DateTime.UtcNow.AddHours(7);
                    existContactCustomer.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                    {
                    e => e.SaleStatus,
                    e => e.Updated,
                    e => e.UpdatedBy
                    };
                    await this.contactCustomerService.UpdateFieldAsync(existContactCustomer, includeProperties);

                }
                else throw new AppException("Không tìm thấy thông tin liên hệ");
            }
            else throw new UnauthorizedAccessException("Không có quyền cập nhật trạng thái sale");
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #region MAPPING YÊU CẦU LIÊN HỆ

        /// <summary>
        /// Lấy thông tin yêu cầu liên hệ
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="mappingRequestId"></param>
        /// <returns></returns>
        [HttpGet("get-mapping-request-info")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetMappingRequestInfo([FromQuery] int contactCustomerId, int mappingRequestId)
        {
            ContactCustomerMappingRequestModel contactCustomerMappingRequestModel = null;
            var mappingRequestInfo = await this.contactCustomerMappingRequestService.GetSingleAsync(e => !e.Deleted
            && e.Active
            && e.ContactCustomerId == contactCustomerId
            && e.Id == mappingRequestId
            );
            if (mappingRequestInfo != null)
            {
                var sourceInfo = await this.sourceTypeService.GetSingleAsync(e => e.Id == mappingRequestInfo.SourceId);
                mappingRequestInfo.SourceName = sourceInfo != null ? sourceInfo.Name : string.Empty;
                var requestInfo = await this.requestTypeService.GetSingleAsync(e => e.Id == mappingRequestInfo.RequestId);
                mappingRequestInfo.RequestName = requestInfo != null ? requestInfo.Name : string.Empty;
                contactCustomerMappingRequestModel = mapper.Map<ContactCustomerMappingRequestModel>(mappingRequestInfo);
            }

            return new AppDomainResult()
            {
                Success = true,
                Data = contactCustomerMappingRequestModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới mapping yêu cầu
        /// </summary>
        /// <param name="requestContactCustomerMappingRequestModel"></param>
        /// <returns></returns>
        [HttpPost("add-mapping-request-info")]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddMappingRequestInfo([FromBody] RequestContactCustomerMappingRequestModel requestContactCustomerMappingRequestModel)
        {
            bool result = false;
            if (requestContactCustomerMappingRequestModel != null)
            {
                var itemAddNew = mapper.Map<ContactCustomerMappingRequests>(requestContactCustomerMappingRequestModel);
                itemAddNew.Created = DateTime.UtcNow.AddHours(7);
                itemAddNew.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemAddNew.Active = true;
                var messageCheckExist = await this.contactCustomerMappingRequestService.GetExistItemMessage(itemAddNew);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerMappingRequestService.CreateAsync(itemAddNew);
            }
            else throw new AppException("Bad Request");


            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Cập nhật thông tin mapping yêu cầu
        /// </summary>
        /// <param name="requestContactCustomerMappingRequestModel"></param>
        /// <returns></returns>
        [HttpPut("update-mapping-request-info/{mappingRequestId}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMappingRequestInfo([FromBody] RequestContactCustomerMappingRequestModel requestContactCustomerMappingRequestModel)
        {
            bool result = false;
            if (requestContactCustomerMappingRequestModel != null)
            {
                var itemUpdate = mapper.Map<ContactCustomerMappingRequests>(requestContactCustomerMappingRequestModel);
                itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.Active = true;
                var messageCheckExist = await this.contactCustomerMappingRequestService.GetExistItemMessage(itemUpdate);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerMappingRequestService.UpdateAsync(itemUpdate);
            }
            else throw new AppException("Bad Request");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Xóa thông tin mapping yêu cầu
        /// </summary>
        /// <param name="mappingRequestId"></param>
        /// <returns></returns>
        [HttpDelete("delete-mapping-request-info/{mappingRequestId}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteMappingRequestInfo(int mappingRequestId)
        {
            bool success = false;
            success = await this.contactCustomerMappingRequestService.DeleteAsync(mappingRequestId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion

        #region QUẢN LÝ GHI CHÚ

        /// <summary>
        /// Lấy thông tin ghi chú
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="contactCustomerNoteId"></param>
        /// <returns></returns>
        [HttpGet("get-contact-customer-note-info")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetContactCustomerNote([FromQuery] int contactCustomerId, int contactCustomerNoteId)
        {
            ContactCustomerNoteModel contactCustomerMappingRequestModel = null;
            var contactCustomerNote = await this.contactCustomerNoteService.GetSingleAsync(e => !e.Deleted
            && e.Active
            && e.ContactCustomerId == contactCustomerId
            && e.Id == contactCustomerNoteId
            );
            if (contactCustomerNote != null)
                contactCustomerMappingRequestModel = mapper.Map<ContactCustomerNoteModel>(contactCustomerNote);

            return new AppDomainResult()
            {
                Success = true,
                Data = contactCustomerMappingRequestModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới thông tin ghi chú
        /// </summary>
        /// <param name="requestContactCustomerNoteModel"></param>
        /// <returns></returns>
        [HttpPost("add-contact-customer-note-info")]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddContactCustomerNote([FromBody] RequestContactCustomerNoteModel requestContactCustomerNoteModel)
        {
            bool result = false;
            if (requestContactCustomerNoteModel != null)
            {
                var itemAddNew = mapper.Map<ContactCustomerNotes>(requestContactCustomerNoteModel);
                itemAddNew.Created = DateTime.UtcNow.AddHours(7);
                itemAddNew.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemAddNew.Active = true;
                var messageCheckExist = await this.contactCustomerNoteService.GetExistItemMessage(itemAddNew);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerNoteService.CreateAsync(itemAddNew);
            }
            else throw new AppException("Bad Request");


            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Cập nhật thông tin ghi chú
        /// </summary>
        /// <param name="requestContactCustomerNoteModel"></param>
        /// <returns></returns>
        [HttpPut("update-contact-customer-note-info/{contactCustomerNoteId}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateContactCustomerNote([FromBody] RequestContactCustomerNoteModel requestContactCustomerNoteModel)
        {
            bool result = false;
            if (requestContactCustomerNoteModel != null)
            {
                var itemUpdate = mapper.Map<ContactCustomerNotes>(requestContactCustomerNoteModel);
                itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.Active = true;
                var messageCheckExist = await this.contactCustomerNoteService.GetExistItemMessage(itemUpdate);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerNoteService.UpdateAsync(itemUpdate);
            }
            else throw new AppException("Bad Request");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Xóa thông tin ghi chú
        /// </summary>
        /// <param name="contactCustomerNoteId"></param>
        /// <returns></returns>
        [HttpDelete("delete-contact-customer-note-info/{contactCustomerNoteId}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteContactCustomerNote(int contactCustomerNoteId)
        {
            bool success = false;
            success = await this.contactCustomerNoteService.DeleteAsync(contactCustomerNoteId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


        #endregion

        #region QUẢN LÝ DỊCH VỤ

        /// <summary>
        /// Lấy thông tin dịch vụ
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="contactCustomerServiceId"></param>
        /// <returns></returns>
        [HttpGet("get-contact-customer-service-info")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetContactCustomerService([FromQuery] int contactCustomerId, int contactCustomerServiceId)
        {
            ContactCustomerServiceModel contactCustomerServiceModel = null;
            var contactCustomerService = await this.contactCustomerServiceTypeService.GetSingleAsync(e => !e.Deleted
            && e.Active
            && e.ContactCustomerId == contactCustomerId
            && e.Id == contactCustomerServiceId
            );
            if (contactCustomerService != null)
                contactCustomerServiceModel = mapper.Map<ContactCustomerServiceModel>(contactCustomerService);

            return new AppDomainResult()
            {
                Success = true,
                Data = contactCustomerServiceModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới thông tin dịch vụ
        /// </summary>
        /// <param name="requestContactCustomerServiceModel"></param>
        /// <returns></returns>
        [HttpPost("add-contact-customer-service-info")]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddContactCustomerService([FromBody] RequestContactCustomerServiceModel requestContactCustomerServiceModel)
        {
            bool result = false;
            if (requestContactCustomerServiceModel != null)
            {
                var itemAddNew = mapper.Map<ContactCustomerServices>(requestContactCustomerServiceModel);
                itemAddNew.Created = DateTime.UtcNow.AddHours(7);
                itemAddNew.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemAddNew.Active = true;
                var messageCheckExist = await this.contactCustomerServiceTypeService.GetExistItemMessage(itemAddNew);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerServiceTypeService.CreateAsync(itemAddNew);
            }
            else throw new AppException("Bad Request");


            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Cập nhật thông tin dịch vụ
        /// </summary>
        /// <param name="requestContactCustomerServiceModel"></param>
        /// <returns></returns>
        [HttpPut("update-contact-customer-service-info/{contactCustomerServiceId}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateContactCustomerService([FromBody] RequestContactCustomerServiceModel requestContactCustomerServiceModel)
        {
            bool result = false;
            if (requestContactCustomerServiceModel != null)
            {
                var itemUpdate = mapper.Map<ContactCustomerServices>(requestContactCustomerServiceModel);
                itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.Active = true;
                var messageCheckExist = await this.contactCustomerServiceTypeService.GetExistItemMessage(itemUpdate);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerServiceTypeService.UpdateAsync(itemUpdate);
            }
            else throw new AppException("Bad Request");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Xóa thông tin dịch vụ
        /// </summary>
        /// <param name="contactCustomerServiceId"></param>
        /// <returns></returns>
        [HttpDelete("delete-contact-customer-service-info/{contactCustomerServiceId}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteContactCustomerService(int contactCustomerServiceId)
        {
            bool success = false;
            success = await this.contactCustomerServiceTypeService.DeleteAsync(contactCustomerServiceId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


        #endregion

        #region QUẢN LÝ THÔNG TIN YÊU CẦU CỦA LIÊN HỆ

        /// <summary>
        /// Lấy thông tin yêu cầu của liên hệ
        /// </summary>
        /// <param name="contactCustomerId"></param>
        /// <param name="contactCustomerSaleRequestId"></param>
        /// <returns></returns>
        [HttpGet("get-contact-customer-sale-request-info")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetContactCustomerSaleRequest([FromQuery] int contactCustomerId, int contactCustomerSaleRequestId)
        {
            ContactCustomerSaleRequestModel contactCustomerSaleRequestModel = null;
            var contactCustomerSaleRequest = await this.contactCustomerSaleRequestService.GetSingleAsync(e => !e.Deleted
            && e.Active
            && e.ContactCustomerId == contactCustomerId
            && e.Id == contactCustomerSaleRequestId
            );
            if (contactCustomerSaleRequest != null)
                contactCustomerSaleRequestModel = mapper.Map<ContactCustomerSaleRequestModel>(contactCustomerSaleRequest);

            return new AppDomainResult()
            {
                Success = true,
                Data = contactCustomerSaleRequestModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới thông tin yêu cầu của liên hệ
        /// </summary>
        /// <param name="requestContactCustomerSaleRequestModel"></param>
        /// <returns></returns>
        [HttpPost("add-contact-customer-sale-request-info")]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddContactCustomerSaleRequest([FromBody] RequestContactCustomerSaleRequestModel requestContactCustomerSaleRequestModel)
        {
            bool result = false;
            if (requestContactCustomerSaleRequestModel != null)
            {
                var itemAddNew = mapper.Map<ContactCustomerSaleRequests>(requestContactCustomerSaleRequestModel);
                itemAddNew.Created = DateTime.UtcNow.AddHours(7);
                itemAddNew.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemAddNew.Active = true;
                var messageCheckExist = await this.contactCustomerSaleRequestService.GetExistItemMessage(itemAddNew);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerSaleRequestService.CreateAsync(itemAddNew);
            }
            else throw new AppException("Bad Request");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Cập nhật thông tin yêu cầu của liên hệ
        /// </summary>
        /// <param name="requestContactCustomerSaleRequestModel"></param>
        /// <returns></returns>
        [HttpPut("update-contact-customer-sale-request-info/{contactCustomerSaleId}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateContactCustomerSaleRequest([FromBody] RequestContactCustomerSaleRequestModel requestContactCustomerSaleRequestModel)
        {
            bool result = false;
            if (requestContactCustomerSaleRequestModel != null)
            {
                var itemUpdate = mapper.Map<ContactCustomerSaleRequests>(requestContactCustomerSaleRequestModel);
                itemUpdate.Updated = DateTime.UtcNow.AddHours(7);
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.Active = true;
                var messageCheckExist = await this.contactCustomerSaleRequestService.GetExistItemMessage(itemUpdate);
                if (!string.IsNullOrEmpty(messageCheckExist))
                    throw new AppException(messageCheckExist);
                result = await this.contactCustomerSaleRequestService.UpdateAsync(itemUpdate);
            }
            else throw new AppException("Bad Request");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = result
            };
        }

        /// <summary>
        /// Xóa thông tin yêu cầu của liên hệ
        /// </summary>
        /// <param name="contactCustomerSaleRequestId"></param>
        /// <returns></returns>
        [HttpDelete("delete-contact-customer-sale-request-info/{contactCustomerSaleRequestId}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteContactCustomerSaleRequest(int contactCustomerSaleRequestId)
        {
            bool success = false;
            success = await this.contactCustomerSaleRequestService.DeleteAsync(contactCustomerSaleRequestId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion

        #region QUẢN LÝ THÔNG TIN FILE

        /// <summary>
        /// Xóa file của yêu cầu liên hệ
        /// </summary>
        /// <param name="contactCustomerFileId"></param>
        /// <returns></returns>
        [HttpDelete("delete-contact-customer-file/{contactCustomerFileId}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteContactCustomerFile(int contactCustomerFileId)
        {
            bool success = false;
            success = await this.contactCustomerFileService.DeleteAsync(contactCustomerFileId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion

        #region LẤY CHỈ SỐ BÁO CÁO

        /// <summary>
        /// Đếm tổng số liên hệ khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-contact-count")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetContactCount()
        {
            SearchReportSource searchReportSource = new SearchReportSource()
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                OrderBy = "YearValue desc"
            };
            int totalTawk = 0;
            int totalPhone = 0;
            int totalForm = 0;
            int totalOther = 0;
            int totalContact = await this.contactCustomerService.CountTotalContact();

            var reportSourcePagedList = await this.reportSourceService.GetPagedListReport(searchReportSource);
            if (reportSourcePagedList != null && reportSourcePagedList.Items.Any())
            {
                totalTawk = reportSourcePagedList.Items.Sum(e => e.TotalTawks ?? 0);
                totalPhone = reportSourcePagedList.Items.Sum(e => e.TotalPhones ?? 0);
                totalForm = reportSourcePagedList.Items.Sum(e => e.TotalForms ?? 0);
                totalOther = reportSourcePagedList.Items.Sum(e => e.TotalOthers ?? 0);
            }
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = new
                {
                    TotalContact = totalContact,
                    TotalTawk = totalTawk,
                    TotalPhone = totalPhone,
                    TotalForm = totalForm,
                    TotalOther = totalOther,
                },
            };
        }

        #endregion


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
