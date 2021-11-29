using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

namespace QuanLy.Core.Controllers
{
    [Route("api/notification-single")]
    [ApiController]
    [Authorize]
    [Description("Quản lý thông báo")]
    public class NotificationSingleController : BaseController<NotificationSingles, NotificationSingleModel, RequestNotificationSingleModel, SearchNotification>
    {
        protected INotificationSingle notificationSingle;
        public NotificationSingleController(IServiceProvider serviceProvider, ILogger<BaseController<NotificationSingles, NotificationSingleModel, RequestNotificationSingleModel, SearchNotification>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<INotificationSingle>();
            this.notificationSingle = serviceProvider.GetRequiredService<INotificationSingle>();
        }

        /// <summary>
        /// Lấy Số thông báo của User
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-number")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<int> GetChilComment()
        {
            var UID = LoginContext.Instance.CurrentUser.UserId;
            return await this.notificationSingle.NumberNotification(UID);

        }


        /// <summary>
        /// Cập nhật thông báo User
        /// </summary>
        /// <returns></returns>
        [HttpPost("update-status")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> UpdateStatus(int Id)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            var UID = LoginContext.Instance.CurrentUser.UserId;
            var Message = await this.notificationSingle.UpdateStaus(Id, UID);
            if (string.IsNullOrEmpty(Message))
            {
                success = true;
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
                throw new KeyNotFoundException(Message);
            appDomainResult.Success = success;
            return appDomainResult;
        }
    }
}
