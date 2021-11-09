using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Model;
using QuanLy.Service;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.WebHooks
{
    [Route("api/web-hook-contact-customer")]
    [ApiController]
    [Description("Web hook thông tin liên hệ")]
    public class WebHookContactCustomerController : ControllerBase
    {
        protected IMapper mapper;
        protected IContactCustomerService contactCustomerService;
        protected IUserService userService;
        protected IRequestTypeService requestTypeService;
        protected IContactCustomerMappingRequestService contactCustomerMappingRequestService;
        protected ISourceTypeService sourceTypeService;
        protected ICampaignSourceService campaignSourceService;
        protected ICampaignMediumService campaignMediumService;
        protected IWebHookFormHistoryService webHookFormHistoryService;
        protected IWebHookTawkHistoryService webHookTawkHistoryService;
        protected IWebHookPhoneHistoryService webHookPhoneHistoryService;
        protected IContactCustomerMappingUserService contactCustomerMappingUserService;
        protected IProjectServiceService projectServiceService;


        protected IConfiguration configuration;
        protected IHubContext<NotificationHub> hubContext;
        public WebHookContactCustomerController(IServiceProvider serviceProvider
            , IMapper mapper
            , IHubContext<NotificationHub> hubContext
            , IConfiguration configuration
            )
        {
            contactCustomerService = serviceProvider.GetRequiredService<IContactCustomerService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            requestTypeService = serviceProvider.GetRequiredService<IRequestTypeService>();
            contactCustomerMappingRequestService = serviceProvider.GetRequiredService<IContactCustomerMappingRequestService>();
            sourceTypeService = serviceProvider.GetRequiredService<ISourceTypeService>();
            campaignMediumService = serviceProvider.GetRequiredService<ICampaignMediumService>();
            campaignSourceService = serviceProvider.GetRequiredService<ICampaignSourceService>();
            webHookFormHistoryService = serviceProvider.GetRequiredService<IWebHookFormHistoryService>();
            webHookPhoneHistoryService = serviceProvider.GetRequiredService<IWebHookPhoneHistoryService>();
            webHookTawkHistoryService = serviceProvider.GetRequiredService<IWebHookTawkHistoryService>();
            contactCustomerMappingUserService = serviceProvider.GetRequiredService<IContactCustomerMappingUserService>();
            projectServiceService = serviceProvider.GetRequiredService<IProjectServiceService>();

            this.mapper = mapper;
            this.hubContext = hubContext;

            this.configuration = configuration;
        }

        #region WEB HOOK FORM

        /// <summary>
        /// Gửi thông tin liên hệ từ form
        /// </summary>
        /// <param name="customerContactRequestFromForm"></param>
        /// <returns></returns>
        [HttpPost("send-request-contact-form")]
        public async Task<AppDomainResult> SendRequestContactFromForm([FromBody] RequestContactCustomerFormModel customerContactRequestFromForm)
        {
            bool success = false;
            int? contactCustomerId = null;
            if (customerContactRequestFromForm == null)
            {
                return new AppDomainResult()
                {
                    Data = false,
                    ResultCode = (int)HttpStatusCode.BadRequest
                };
            }
            // Lấy thông tin liên hệ khách theo ngày và số điện thoại
            var existUser = await this.userService.GetSingleAsync(e => e.Phone == customerContactRequestFromForm.phone);

            // Lấy thông tin liên hệ đã tồn tại trong ngày
            var existContactCustomer = await this.contactCustomerService.GetExistContactCustomer(customerContactRequestFromForm.phone, string.Empty);

            var sourceFormType = await this.sourceTypeService.GetSingleAsync(e => e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.FORM.ToString());
            if (sourceFormType == null)
            {
                sourceFormType = new SourceTypes()
                {
                    Deleted = false,
                    Active = true,
                    Code = CatalogueEnums.ContactCustomerSourceCatalogue.FORM.ToString(),
                    Name = "Form",
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = "Web hook API",
                };
                await this.sourceTypeService.CreateAsync(sourceFormType);
            }

            // Thêm thông tin liên hệ
            if (!string.IsNullOrEmpty(customerContactRequestFromForm.phone))
            {
                // Kiểm tra thông tin liên đã tồn tại trong ngày chưa?
                // TH1: Có => Cập nhật lại yêu cầu của thông tin liên hệ
                if (existContactCustomer != null)
                {
                    existContactCustomer.Updated = DateTime.UtcNow.AddHours(7);

                    Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                    {
                        e => e.Updated
                    };
                    await this.contactCustomerService.UpdateFieldAsync(existContactCustomer, includeProperties);
                    contactCustomerId = existContactCustomer.Id;
                    success = true;
                }
                // TH2: Chưa có => Thêm mới thông tin liên hệ.
                else
                {
                    // Nếu có thông tin khách => cập nhật thông tin khách
                    ContactCustomers contactCustomer = null;
                    if (existUser != null)
                    {
                        contactCustomer = new ContactCustomers
                        {
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            Deleted = false,
                            Active = true,
                            Phone = existUser.Phone,
                            FullName = existUser.UserFullName,
                            Email = existUser.Email,
                            UserId = existUser.Id,
                            BirthDate = existUser.BirthDate,
                            Gender = existUser.Gender,
                            Address = existUser.Address,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                        };
                    }
                    else
                    {
                        contactCustomer = new ContactCustomers
                        {
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            Deleted = false,
                            Active = true,
                            Phone = customerContactRequestFromForm.phone,
                            FullName = customerContactRequestFromForm.full_name,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                            Email = customerContactRequestFromForm.email,
                        };
                    }

                    success = await this.contactCustomerService.CreateAsync(contactCustomer);
                    if (success)
                        contactCustomerId = contactCustomer.Id;
                }
                // Thêm mới yêu cầu
                if (!string.IsNullOrEmpty(customerContactRequestFromForm.request) && contactCustomerId.HasValue)
                {
                    RequestTypes requestInfo = null;
                    var requestInfos = await this.requestTypeService.GetAsync(e => customerContactRequestFromForm.request.Contains(e.Name));
                    if (requestInfos == null || !requestInfos.Any())
                    {
                        RequestTypes requestTypes = new RequestTypes()
                        {
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            Code = RandomUtilities.RandomString(8),
                            Name = customerContactRequestFromForm.request,
                            Deleted = false,
                            Active = true,
                        };
                        await this.requestTypeService.CreateAsync(requestTypes);
                        requestInfo = requestTypes;
                    }
                    else requestInfo = requestInfos.FirstOrDefault();

                    if (requestInfo != null)
                    {
                        //var currentRequestInfos = await ContactCustomerMappingRequestTable.GetMappingRequests(contactCustomerId.Value, requestInfo.ID);
                        var currentRequestInfo = await this.contactCustomerMappingRequestService.GetSingleAsync(e => !e.Deleted && e.Active
                        && e.ContactCustomerId == contactCustomerId.Value
                        && e.RequestId == requestInfo.Id
                        && e.SourceId == sourceFormType.Id
                        );
                        if (currentRequestInfo == null)
                        {
                            ContactCustomerMappingRequests contactCustomerMappingRequests = new ContactCustomerMappingRequests()
                            {
                                ContactCustomerId = contactCustomerId,
                                Deleted = false,
                                Active = true,
                                Description = customerContactRequestFromForm.request_description,
                                RequestId = requestInfo.Id,
                                CreatedBy = "Api",
                                Created = DateTime.UtcNow.AddHours(7),
                                SourceId = sourceFormType.Id,
                                SourceName = "Form"
                            };
                            await contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequests);
                        }
                    }
                }
                // Nếu không có thông tin yêu cầu, thêm mặc định thông tin cho form
                else
                {
                    if (contactCustomerId.HasValue)
                    {
                        ContactCustomerMappingRequests contactCustomerMappingRequests = new ContactCustomerMappingRequests()
                        {
                            ContactCustomerId = contactCustomerId,
                            Deleted = false,
                            Active = true,
                            Description = customerContactRequestFromForm.request_description,
                            RequestId = null,
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            SourceId = sourceFormType.Id,
                            SourceName = "Form"
                        };
                        await contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequests);
                    }
                }
                // Kiểm tra Tên nguồn chi tiết đã tồn tại chưa
                // Có => Lấy thông tin nguồn chi tiết
                // Không => Tạo mới => Trả ra thông tin lưu vào web hook
                int? campaginSourceId = null;
                var existCategorySourceInfo = await this.campaignSourceService.GetSingleAsync(e => !e.Deleted
                && e.Active && e.Name.ToLower().Contains(customerContactRequestFromForm.utm_campagin_source.ToLower()));
                if (existCategorySourceInfo == null)
                {
                    CampaignSources campaignSource = new CampaignSources()
                    {
                        CreatedBy = "Api",
                        Created = DateTime.UtcNow.AddHours(7),
                        Deleted = false,
                        Active = true,
                        Code = RandomUtilities.RandomString(8),
                        Name = customerContactRequestFromForm.utm_campagin_source
                    };
                    await this.campaignSourceService.CreateAsync(campaignSource);
                    campaginSourceId = campaignSource.Id;
                }
                else campaginSourceId = existCategorySourceInfo.Id;

                // Kiểm tra tên loại chiến dịch đã tồn tại chưa
                // Có => Lấy thông tin loại chiến dịch
                // Không => Tạo mới => Trả ra thông tin loại chiến dịch
                int? campaginMediumId = null;
                var existCategoryMediumInfo = await this.campaignMediumService.GetSingleAsync(e => !e.Deleted && e.Active
                && e.Name.ToLower().Contains(customerContactRequestFromForm.utm_campagin_medium.ToLower())
                );
                if (existCategoryMediumInfo == null)
                {
                    CampaignMediums campaignMedium = new CampaignMediums()
                    {
                        CreatedBy = "Api",
                        Created = DateTime.UtcNow.AddHours(7),
                        Deleted = false,
                        Active = true,
                        Code = RandomUtilities.RandomString(8),
                        Name = customerContactRequestFromForm.utm_campagin_medium,
                    };
                    await campaignMediumService.CreateAsync(campaignMedium);
                    campaginMediumId = campaignMedium.Id;
                }
                else campaginMediumId = existCategoryMediumInfo.Id;


                // Thêm mới lịch sử webhook
                WebHookFormHistories webHookFormHistory = new WebHookFormHistories()
                {
                    Phone = customerContactRequestFromForm.phone,
                    UserFullName = customerContactRequestFromForm.full_name,
                    SourceId = sourceFormType.Id,
                    SourceName = "Form",
                    Email = customerContactRequestFromForm.email,
                    CampaignSourceId = campaginSourceId,
                    CampaignMediumId = campaginMediumId,
                    CampaignSourceName = customerContactRequestFromForm.utm_campagin_source,
                    CampaignTerm = customerContactRequestFromForm.utm_campaign_term,
                    CampaignContent = customerContactRequestFromForm.utm_campaign_content,
                    CampaignMediumName = customerContactRequestFromForm.utm_campagin_medium,
                    CampaignName = customerContactRequestFromForm.utm_campaign_name,
                    CreatedBy = "Api",
                    PersonChargeName = customerContactRequestFromForm.utm_nguoi_phu_trach,
                    PersonChargePhone = customerContactRequestFromForm.utm_phone_nguoi_phu_trach,
                    ContactCustomerId = contactCustomerId,
                    CampaignId = customerContactRequestFromForm.utm_campagin_id,
                    FormPreviousUrl = customerContactRequestFromForm.previous_form_url,
                    ToDestinationUrl = customerContactRequestFromForm.to_destination,
                    FromAnchorUrl = customerContactRequestFromForm.from_anchor,
                    FormUrl = customerContactRequestFromForm.form_url,
                    Created = DateTime.UtcNow.AddHours(7)
                };
                await this.webHookFormHistoryService.CreateAsync(webHookFormHistory);
                // GỬi notify tới trang liên hệ khách hàng
                await hubContext.Clients.All.SendAsync(Contants.NOTIFY_SEND_CONTACT_CUSTOMER);

                return new AppDomainResult()
                {
                    ResultCode = success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError,
                    Data = success,
                    ResultMessage = success ? "Thành công" : "Thất bại"
                };

            }

            else return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.BadRequest,
                Data = false,
                ResultMessage = "Vui lòng nhập thông tin SDT!"
            };
        }

        #endregion

        #region WEB HOOK TAWK

        /// <summary>
        /// Khách gửi yêu cầu từ tawk to
        /// </summary>
        /// <param name="customerContactRequest"></param>
        /// <returns></returns>
        [HttpPost("send-request-contact-tawk")]
        public async Task<AppDomainResult> SendRequestContactFromTawkTo([FromBody] RequestContactCustomerTawkModel customerContactRequest)
        {
            AppDomainResult dataAPI = new AppDomainResult();
            bool success = false;
            // Lấy thông tin user theo username (phone)
            if (customerContactRequest == null)
            {
                return new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Data = false,
                    ResultMessage = "Thất bại"
                };
            }

            if (customerContactRequest.CustomerContactTawkToRequests != null
            && customerContactRequest.CustomerContactTawkToRequests.Any()
            && customerContactRequest.CustomerContactTawkToRequests.Count >= 4
            )
            {
                customerContactRequest.FullName = customerContactRequest.CustomerContactTawkToRequests[0].answer;
                customerContactRequest.Email = customerContactRequest.CustomerContactTawkToRequests[1].answer;
                customerContactRequest.Phone = customerContactRequest.CustomerContactTawkToRequests[2].answer;
                string requestSelected_s = customerContactRequest.CustomerContactTawkToRequests[3].answer;
                var listRequestSelected = requestSelected_s.Split(',').ToList();
                // Lấy ra tất cả thông tin yêu cầu được chọn
                List<RequestTypes> requestTypeSelecteds = new List<RequestTypes>();
                foreach (var requestSelected in listRequestSelected)
                {
                    var requestInfos = await this.requestTypeService.GetAsync(e => !e.Deleted && e.Active
                    && (string.IsNullOrEmpty(requestSelected) || (e.Code.ToLower().Contains(requestSelected.Trim().ToLower())
                    || e.Name.ToLower().Contains(requestSelected.Trim().ToLower())
                    || e.Description.ToLower().Contains(requestSelected.Trim().ToLower())
                    )));
                    if (requestInfos != null && requestInfos.Any())
                        requestTypeSelecteds.Add(requestInfos.FirstOrDefault());
                    else
                    {
                        RequestTypes requestType = new RequestTypes()
                        {
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            Code = RandomUtilities.RandomString(8),
                            Name = requestSelected,
                            Deleted = false,
                            Active = true,
                        };
                        await this.requestTypeService.CreateAsync(requestType);
                    }
                }
                int contactCustomerId = 0;
                // Kiểm tra thông tin liên hệ đã có trong ngày chưa?
                var existContactCustomer = await this.contactCustomerService.GetExistContactCustomer(customerContactRequest.Phone, string.Empty);

                // Lấy thông tin liên hệ khách theo ngày và số điện thoại
                var existUser = await this.userService.GetSingleAsync(e => e.Phone == customerContactRequest.Phone);

                // Lấy thông tin source type tawk
                var sourceTypeTawk = await this.sourceTypeService.GetSingleAsync(e => !e.Deleted && e.Active
                && e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString());
                if (sourceTypeTawk == null)
                {
                    sourceTypeTawk = new SourceTypes()
                    {
                        Deleted = false,
                        Active = true,
                        Code = CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString(),
                        Name = "Tawk to",
                        Created = DateTime.UtcNow.AddHours(7),
                        CreatedBy = "Api",
                    };
                    await this.sourceTypeService.CreateAsync(sourceTypeTawk);
                }

                // TH1: Có => Update thêm thông tin yêu cầu theo phiếu đã có
                if (existContactCustomer != null)
                {
                    // Lấy ra list thông tin yêu cầu đã có
                    var listCurrentMappingRequests = await this.contactCustomerMappingRequestService.GetAsync(e => !e.Deleted && e.Active
                        && e.ContactCustomerId == existContactCustomer.Id
                        );
                    if (requestTypeSelecteds != null && requestTypeSelecteds.Any())
                    {
                        foreach (var item in requestTypeSelecteds)
                        {
                            var existContactCustomerMapping = listCurrentMappingRequests.Where(e => e.RequestId == item.Id).FirstOrDefault();
                            if (existContactCustomerMapping == null)
                            {
                                ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                                {
                                    RequestId = item.Id,
                                    Deleted = false,
                                    Active = true,
                                    ContactCustomerId = existContactCustomer.Id,
                                    CreatedBy = "Api",
                                    Created = DateTime.UtcNow.AddHours(7),
                                    SourceId = sourceTypeTawk.Id,
                                    SourceName = "Tawk to",
                                };
                                await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                            }
                            else continue;
                        }
                    }
                    else
                    {
                        if (listCurrentMappingRequests == null || !listCurrentMappingRequests.Any(e => e.SourceId == (int)CatalogueEnums.ContactCustomerSourceCatalogue.TAWK))
                        {
                            ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                            {
                                RequestId = null,
                                Deleted = false,
                                Active = true,
                                ContactCustomerId = existContactCustomer.Id,
                                CreatedBy = "Api",
                                Created = DateTime.UtcNow.AddHours(7),
                                SourceId = sourceTypeTawk.Id,
                                SourceName = "Tawk to"
                            };
                            await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                        }

                    }

                    existContactCustomer.Updated = DateTime.UtcNow.AddHours(7);

                    Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                    {
                        e => e.Updated
                    };
                    await this.contactCustomerService.UpdateFieldAsync(existContactCustomer, includeProperties);

                    success = true;
                    contactCustomerId = existContactCustomer.Id;
                }
                // TH2: Chưa có thông tin trong ngày => Thêm mới thông tin liên hệ khách hàng.
                else
                {
                    ContactCustomers contactCustomer = null;
                    if (existUser != null)
                    {
                        contactCustomer = new ContactCustomers()
                        {
                            FullName = existUser.UserFullName,
                            Email = existUser.Email,
                            Phone = customerContactRequest.Phone,
                            UserId = existUser.Id,
                            BirthDate = existUser.BirthDate,
                            Address = existUser.Address,
                            Gender = existUser.Gender,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = "Api",
                        };
                    }
                    else
                    {
                        contactCustomer = new ContactCustomers()
                        {
                            FullName = customerContactRequest.FullName,
                            Email = customerContactRequest.Email,
                            Phone = customerContactRequest.Phone,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                            Created = DateTime.UtcNow.AddHours(7),
                            CreatedBy = "Api",
                        };
                    }
                    // Lưu thông tin liên hệ của khách hàng
                    success = await this.contactCustomerService.CreateAsync(contactCustomer);
                    if (success)
                    {
                        contactCustomerId = contactCustomer.Id;
                        if (requestTypeSelecteds != null && requestTypeSelecteds.Any())
                        {
                            foreach (var item in requestTypeSelecteds)
                            {
                                ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                                {
                                    RequestId = item.Id,
                                    Deleted = false,
                                    Active = true,
                                    ContactCustomerId = contactCustomer.Id,
                                    CreatedBy = "Api",
                                    Created = DateTime.UtcNow.AddHours(7),
                                    SourceId = sourceTypeTawk.Id,
                                    SourceName = "Tawk to"
                                };
                                await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                            }
                        }
                        else
                        {
                            ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                            {
                                RequestId = null,
                                Deleted = false,
                                Active = true,
                                ContactCustomerId = contactCustomer.Id,
                                CreatedBy = "Api",
                                Created = DateTime.UtcNow.AddHours(7),
                                SourceId = sourceTypeTawk.Id,
                                SourceName = "Tawk to"
                            };
                            await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                        }

                    }
                }


                // Thêm mới lịch sử webhook
                WebHookTawkHistories webHookTawkHistory = new WebHookTawkHistories()
                {
                    Phone = customerContactRequest.Phone,
                    CustomerFullName = customerContactRequest.FullName,
                    UserFullName = customerContactRequest.FullName,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    ContactCustomerId = contactCustomerId,
                    Email = customerContactRequest.Email,
                    TawkType = (int)CatalogueEnums.TawkToType.SubmitPrechat,
                    CreatedBy = "Api",
                    Created = DateTime.UtcNow.AddHours(7)
                };
                await webHookTawkHistoryService.CreateAsync(webHookTawkHistory);
            }
            // GỬi notify tới trang liên hệ khách hàng
            await hubContext.Clients.All.SendAsync(Contants.NOTIFY_SEND_CONTACT_CUSTOMER);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Data = success,
                ResultMessage = success ? "Thành công" : "Thất bại"
            };
        }

        /// <summary>
        /// Khách gửi yêu cầu
        /// </summary>
        /// <param name="customerContactRequest"></param>
        /// <returns></returns>
        [HttpPost("start-chat-web-hook")]
        public async Task<AppDomainResult> StartChatFromWebHook([FromBody] WebHookTawkRequestModel customerContactRequest)
        {
            if (customerContactRequest == null)
            {
                return new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Data = true,
                    ResultMessage = "Thất bại"
                };
            }
            var propertyId = customerContactRequest.property != null ? customerContactRequest.property.id : string.Empty;
            // Kiểm tra đã tồn tại thông tin liên hệ trong ngày theo email
            ContactCustomers contactCustomer = null;
            if (customerContactRequest.visitor != null)
            {
                var exitContactCustomer = await this.contactCustomerService.GetExistContactCustomer(string.Empty, customerContactRequest.visitor.email);
                if (exitContactCustomer != null)
                    contactCustomer = exitContactCustomer;
            }
            // Lấy thông tin source Tawk
            var sourceTypeTawk = await this.sourceTypeService.GetSingleAsync(e => e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString());
            if (sourceTypeTawk == null)
            {
                sourceTypeTawk = new SourceTypes()
                {
                    Deleted = false,
                    Active = true,
                    Code = CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString(),
                    Name = "Tawk to",
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = "Api",
                };
                await this.sourceTypeService.CreateAsync(sourceTypeTawk);
            }

            WebHookTawkHistories webHookTawkHistory = null;
            if (contactCustomer != null)
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.Start,
                    Email = contactCustomer.Email,
                    UserFullName = contactCustomer.FullName,
                    CustomerFullName = contactCustomer.FullName,
                    ContactCustomerId = contactCustomer.Id,
                    Deleted = false,
                    Active = true,
                };
            }
            else
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.Start,
                    Email = customerContactRequest.visitor != null ? customerContactRequest.visitor.email : string.Empty,
                    UserFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    CustomerFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    Deleted = false,
                    Active = true,
                };
            }
            await this.webHookTawkHistoryService.CreateAsync(webHookTawkHistory);


            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Data = true,
                ResultMessage = "Thành công"
            };
        }

        /// <summary>
        /// Kết thúc trò chuyện
        /// </summary>
        /// <param name="customerContactRequest"></param>
        /// <returns></returns>
        [HttpPost("end-chat-web-hook")]
        public async Task<AppDomainResult> EndChatFromWebHook([FromBody] WebHookTawkRequestModel customerContactRequest)
        {
            if (customerContactRequest == null)
            {
                return new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Data = true,
                    ResultMessage = "Thất bại"
                };
            }

            var propertyId = customerContactRequest.property != null ? customerContactRequest.property.id : string.Empty;
            // Kiểm tra đã tồn tại thông tin liên hệ trong ngày theo email
            ContactCustomers contactCustomer = null;
            if (customerContactRequest.visitor != null)
            {
                var exitContactCustomer = await contactCustomerService.GetExistContactCustomer(string.Empty, customerContactRequest.visitor.email);
                if (exitContactCustomer != null)
                    contactCustomer = exitContactCustomer;
            }
            WebHookTawkHistories webHookTawkHistory = null;

            // Lấy thông tin source Tawk
            var sourceTypeTawk = await this.sourceTypeService.GetSingleAsync(e => e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString());
            if (sourceTypeTawk == null)
            {
                sourceTypeTawk = new SourceTypes()
                {
                    Deleted = false,
                    Active = true,
                    Code = CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString(),
                    Name = "Tawk to",
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = "Api",
                };
                await this.sourceTypeService.CreateAsync(sourceTypeTawk);
            }

            if (contactCustomer != null)
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.End,
                    Email = contactCustomer.Email,
                    UserFullName = contactCustomer.FullName,
                    CustomerFullName = contactCustomer.FullName,
                    ContactCustomerId = contactCustomer.Id,
                    Deleted = false,
                    Active = true
                };
            }
            else
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.End,
                    Email = customerContactRequest.visitor != null ? customerContactRequest.visitor.email : string.Empty,
                    UserFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    CustomerFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    Deleted = false,
                    Active = true
                };
            }
            await this.webHookTawkHistoryService.CreateAsync(webHookTawkHistory);

            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Data = true,
                ResultMessage = "Thành công"
            };
        }

        /// <summary>
        /// Tạo vé trò chuyện
        /// </summary>
        /// <param name="customerContactRequest"></param>
        /// <returns></returns>
        [HttpPost("create-ticket-web-hook")]
        public async Task<AppDomainResult> CreateTicketChatFromWebHook([FromBody] WebHookTawkRequestModel customerContactRequest)
        {
            if (customerContactRequest == null)
            {
                return new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Data = true,
                    ResultMessage = "Thất bại"
                };
            }
            var propertyId = customerContactRequest.property != null ? customerContactRequest.property.id : string.Empty;
            // Kiểm tra đã tồn tại thông tin liên hệ trong ngày theo email
            ContactCustomers contactCustomer = null;
            if (customerContactRequest.visitor != null)
            {
                var exitContactCustomer = await contactCustomerService.GetExistContactCustomer(string.Empty, customerContactRequest.visitor.email);
                if (exitContactCustomer != null)
                    contactCustomer = exitContactCustomer;
            }
            WebHookTawkHistories webHookTawkHistory = null;
            // Lấy thông tin source Tawk
            var sourceTypeTawk = await this.sourceTypeService.GetSingleAsync(e => e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString());
            if (sourceTypeTawk == null)
            {
                sourceTypeTawk = new SourceTypes()
                {
                    Deleted = false,
                    Active = true,
                    Code = CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString(),
                    Name = "Tawk to",
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = "Api",
                };
                await this.sourceTypeService.CreateAsync(sourceTypeTawk);
            }
            if (contactCustomer != null)
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.End,
                    Email = contactCustomer.Email,
                    UserFullName = contactCustomer.FullName,
                    CustomerFullName = contactCustomer.FullName,
                    ContactCustomerId = contactCustomer.Id,
                    Deleted = false,
                    Active = true
                };
            }
            else
            {
                webHookTawkHistory = new WebHookTawkHistories()
                {
                    CreatedBy = "Api Web Hook",
                    Created = DateTime.UtcNow.AddHours(7),
                    TawkChatId = customerContactRequest.chatId,
                    TawkMessagingId = propertyId,
                    SourceId = sourceTypeTawk.Id,
                    SourceName = "Tawk to",
                    TawkProperty = JsonConvert.SerializeObject(customerContactRequest.property),
                    TawkType = (int)CatalogueEnums.TawkToType.End,
                    Email = customerContactRequest.visitor != null ? customerContactRequest.visitor.email : string.Empty,
                    UserFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    CustomerFullName = customerContactRequest.visitor != null ? customerContactRequest.visitor.name : string.Empty,
                    Deleted = false,
                    Active = true
                };
            }
            await this.webHookTawkHistoryService.CreateAsync(webHookTawkHistory);


            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Data = true,
                ResultMessage = "Thành công"
            };
        }

        #endregion

        #region WEB HOOK PHONE

        /// <summary>
        /// Khách gửi yêu cầu thông qua tổng đài
        /// </summary>
        /// <param name="contactCustomerPhoneRequest"></param>
        /// <returns></returns>
        [HttpPost("send-request-contact-phone")]
        public async Task<AppDomainResult> SendRequestContactFromPhone([FromBody] WebHookContactCustomerPhoneModel contactCustomerPhoneRequest)
        {
            if (contactCustomerPhoneRequest == null)
            {
                return new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Data = false
                };
            }
            var statusRegex = Regex.Replace(contactCustomerPhoneRequest.Status, @"\s+", "");

            // Lưu thông tin xuống phần liên hệ khách hàng
            // DK: Cuộc gọi Inbound (gọi vào), Status = Down (Đã gác máy)
            if ((statusRegex == CatalogueEnums.WebHookContactCustomerStatus.Down.ToString()
                || statusRegex == CatalogueEnums.WebHookContactCustomerStatus.Ringing.ToString())
                && contactCustomerPhoneRequest.Direction == CatalogueEnums.WebHookDirectionType.Inbound.ToString()
                )
            {
                bool result = false;
                int contactCustomerId = 0;
                // Kiểm tra thông tin liên hệ đã có trong ngày chưa?
                var existContactCustomer = await this.contactCustomerService.GetExistContactCustomer(contactCustomerPhoneRequest.CallNumber, string.Empty);

                // Lấy thông tin khách hàng theo số điện thoại
                var existUser = await this.userService.GetSingleAsync(e => !e.Deleted && e.Phone == contactCustomerPhoneRequest.CallNumber);
                // Lấy thông tin source Tawk
                var sourceTypePhone = await this.sourceTypeService.GetSingleAsync(e => e.Code == CatalogueEnums.ContactCustomerSourceCatalogue.PHONE.ToString());
                if (sourceTypePhone == null)
                {
                    sourceTypePhone = new SourceTypes()
                    {
                        Deleted = false,
                        Active = true,
                        Code = CatalogueEnums.ContactCustomerSourceCatalogue.PHONE.ToString(),
                        Name = "Tổng đài",
                        Created = DateTime.UtcNow.AddHours(7),
                        CreatedBy = "Api",
                    };
                    await this.sourceTypeService.CreateAsync(sourceTypePhone);
                }

                if (existContactCustomer != null)
                {
                    // Lấy ra list thông tin yêu cầu đã có
                    var listCurrentMappingRequests = await this.contactCustomerMappingRequestService.GetAsync(e => !e.Deleted && e.Active
                    && (e.ContactCustomerId == existContactCustomer.Id));
                    if (listCurrentMappingRequests == null || !listCurrentMappingRequests
                        .Any(e => e.SourceId == sourceTypePhone.Id))
                    {
                        ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                        {
                            Deleted = false,
                            Active = true,
                            ContactCustomerId = existContactCustomer.Id,
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                            SourceId = sourceTypePhone.Id,
                            SourceName = "Tổng đài",
                            RequestId = null,
                        };
                        await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                    }
                    existContactCustomer.Updated = DateTime.UtcNow.AddHours(7);
                    Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                    {
                        e => e.Updated
                    };
                    await this.contactCustomerService.UpdateFieldAsync(existContactCustomer, includeProperties);
                    contactCustomerId = existContactCustomer.Id;

                    result = true;
                }
                else
                {
                    ContactCustomers contactCustomer = null;
                    // Kiểm tra khách hàng đã có thông tin chưa?
                    // Có => Cập nhật theo thông tin khách
                    if (existUser != null)
                    {
                        contactCustomer = new ContactCustomers()
                        {
                            Phone = contactCustomerPhoneRequest.CallNumber,
                            FullName = existUser.UserFullName,
                            Address = existUser.Address,
                            Gender = existUser.Gender,
                            BirthDate = existUser.BirthDate,
                            Email = existUser.Email,
                            UserId = existUser.Id,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                        };
                    }
                    // Chưa => Lưu liên hệ khách
                    else
                    {
                        contactCustomer = new ContactCustomers()
                        {
                            Phone = contactCustomerPhoneRequest.CallNumber,
                            FullName = string.Empty,
                            Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                            CreatedBy = "Api",
                            Created = DateTime.UtcNow.AddHours(7),
                        };
                    }
                    result = await this.contactCustomerService.CreateAsync(contactCustomer);
                    contactCustomerId = contactCustomer.Id;
                    ContactCustomerMappingRequests contactCustomerMappingRequest = new ContactCustomerMappingRequests()
                    {
                        Deleted = false,
                        Active = true,
                        ContactCustomerId = contactCustomer.Id,
                        CreatedBy = "Api",
                        Created = DateTime.UtcNow.AddHours(7),
                        SourceId = sourceTypePhone.Id,
                        SourceName = "Tổng đài",
                        RequestId = null,
                    };
                    await this.contactCustomerMappingRequestService.CreateAsync(contactCustomerMappingRequest);
                }

                if (result)
                {
                    // Kiểm tra web hook đã tồn tại trong ngày chưa
                    if (statusRegex == CatalogueEnums.WebHookContactCustomerStatus.Ringing.ToString())
                    {
                        var existWebHookRinging = await this.webHookPhoneHistoryService.GetExistWebHookPhoneAsync(contactCustomerPhoneRequest.CallNumber, CatalogueEnums.WebHookContactCustomerStatus.Ringing.ToString());
                        if (existWebHookRinging != null)
                        {
                            return new AppDomainResult()
                            {
                                Data = true,
                                ResultCode = (int)HttpStatusCode.OK
                            };
                        }
                    }

                    // Lấy thông tin account theo đầu số tổng đài nếu có
                    var mappingUserInfo = await this.contactCustomerMappingUserService.GetSingleAsync(e => !e.Deleted && e.Active
                    && e.MappingCode.ToLower().Contains(contactCustomerPhoneRequest.ReceiptNumber.ToLower())
                    );
                    int? userId = null;
                    string userFullName = string.Empty;
                    if (mappingUserInfo != null)
                    {
                        userId = mappingUserInfo != null ? mappingUserInfo.UserId : null;
                        var userInfo = await this.userService.GetByIdAsync(userId ?? 0);
                        if (userInfo != null)
                            userFullName = userInfo.UserFullName + " (" + userInfo.Phone + ")";
                    }
                    // Lưu thông tin web hook post = phone
                    WebHookPhoneHistories webHookPhoneHistory = new WebHookPhoneHistories()
                    {
                        ApiKey = contactCustomerPhoneRequest.ApiKey,
                        CallName = contactCustomerPhoneRequest.CallName,
                        CallNumber = contactCustomerPhoneRequest.CallNumber,
                        Direction = contactCustomerPhoneRequest.Direction,
                        Key = contactCustomerPhoneRequest.Key,
                        KeyRinging = contactCustomerPhoneRequest.KeyRinging,
                        Message = contactCustomerPhoneRequest.Message,
                        NumberPBX = contactCustomerPhoneRequest.NumberPBX,
                        Status = statusRegex,
                        TotalTimeCall = contactCustomerPhoneRequest.Data != null ? contactCustomerPhoneRequest.Data.TotalTimeCall : string.Empty,
                        LinkFile = contactCustomerPhoneRequest.Data != null ? contactCustomerPhoneRequest.Data.LinkFile : string.Empty,
                        RealTimeCall = contactCustomerPhoneRequest.Data != null ? contactCustomerPhoneRequest.Data.RealTimeCall : string.Empty,
                        ReceiptNumber = contactCustomerPhoneRequest.ReceiptNumber,
                        CreatedBy = "Api",
                        Created = DateTime.UtcNow.AddHours(7),
                        SourceId = sourceTypePhone.Id,
                        SourceName = "Tổng đài",
                        ContactCustomerId = contactCustomerId,
                        //ModifiedDate = DateTime.UtcNow.AddHours(7),
                        UserId = existUser.Id,
                        UserFullName = userFullName
                    };
                    await this.webHookPhoneHistoryService.CreateAsync(webHookPhoneHistory);

                    // GỬi notify tới trang liên hệ khách hàng
                    await hubContext.Clients.All.SendAsync(Contants.NOTIFY_SEND_CONTACT_CUSTOMER);
                    return new AppDomainResult()
                    {
                        Data = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    return new AppDomainResult()
                    {
                        Data = false,
                        ResultCode = (int)HttpStatusCode.InternalServerError
                    };
                }
            }


            return new AppDomainResult()
            {
                Data = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion


        #region test onesignal

        [HttpPost("send-one-signal")]
        public async Task<IActionResult> CreateNotification(string title, string content, List<string> PlayerIds)
        {
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            string Url = "https://zingnews.vn/";
            string result = await OneSignalPushNotification.PushNotification(title, content,PlayerIds, AppId, AppSecret, Url);
            return Ok(result);
        }

        //[HttpPost("send-one-signal-2")]
        //public async Task<IActionResult> CreateNotification2(string title, string content, List<string> External)
        //{
        //    var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
        //    var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
        //    string Url = "https://zingnews.vn/";
        //    var result = await OneSignalPushNotification.PushNotification2(title, content, External, AppId, AppSecret, Url);
        //    return Ok(result);
        //}
        #endregion
    }
}
