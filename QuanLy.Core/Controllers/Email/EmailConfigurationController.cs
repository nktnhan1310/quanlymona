using App.Core.Entities.Configuration;
using App.Core.Extensions;
using App.Core.Interface.Services.Configuration;
using App.Core.Models;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers
{
    [Route("api/email-configuration")]
    [ApiController]
    [Description("Cấu hình email server")]
    [Authorize]
    public class EmailConfigurationController : ControllerBase
    {
        private readonly ILogger<EmailConfigurationController> logger;
        private readonly IEmailConfigurationCoreService emailConfigurationService;
        private readonly IMapper mapper;
        public EmailConfigurationController(IServiceProvider serviceProvider, ILogger<EmailConfigurationController> logger, IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            emailConfigurationService = serviceProvider.GetRequiredService<IEmailConfigurationCoreService>();
        }

        /// <summary>
        /// Lấy thông tin cấu hình email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(new string[] { CoreContants.AddNew, CoreContants.View })]
        public async Task<AppDomainResult> GetEmailConfiguration()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            // Lấy thông tin cấu hình
            var configurations = await this.emailConfigurationService.GetAsync(e => !e.Deleted && e.Active, e => new EmailConfigurationCores()
            {
                Id = e.Id,
                Deleted = e.Deleted,
                Active = e.Active,
                Email = e.Email,
                EnableSsl = e.EnableSsl,
                Port = e.Port,
                DisplayName = e.DisplayName,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                SmtpServer = e.SmtpServer,
                ConnectType = e.ConnectType,
                ItemSendCount = e.ItemSendCount,
                TimeSend = e.TimeSend,
                UserName = e.UserName,
                Password = string.Empty

            });
            if (configurations != null && configurations.Any())
            {
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = mapper.Map<EmailConfigurationCoreModel>(configurations.FirstOrDefault())
                };
            }
            // Chưa có cấu hình => tạo mới cấu hình
            else
            {
                EmailConfigurationCores emailConfiguration = new EmailConfigurationCores()
                {
                    Deleted = false,
                    Active = true,
                    Created = DateTime.Now,
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                    SmtpServer = string.Empty,
                    DisplayName = string.Empty,
                    Password = string.Empty,
                    UserName = string.Empty,
                };
                bool success = await this.emailConfigurationService.CreateAsync(emailConfiguration);

                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = mapper.Map<EmailConfigurationCoreModel>(emailConfiguration)
                };
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật cấu hình email
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateEmailConfiguration([FromBody] EmailConfigurationCoreModel emailConfigurationModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                emailConfigurationModel.EncryptPassword();
                var emailConfiguration = mapper.Map<EmailConfigurationCores>(emailConfigurationModel);
                bool success = await this.emailConfigurationService.UpdateAsync(emailConfiguration);
                if (success)
                {
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK,
                    };
                }
                else throw new Exception("Lỗi trong quá trình xử lý!");
            }
            else throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }
    }
}
