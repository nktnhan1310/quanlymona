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
using QuanLy.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Configuration
{
    [Route("api/device-browser")]
    [ApiController]
    [Authorize]
    public class DeviceBrowsersController : BaseController<DeviceBrowsers, DeviceBrowserModel, RequestDeviceBrowserModel, BaseSearch>
    {
        protected IDeviceBrowserService deviceBrowserService;
        public DeviceBrowsersController(IServiceProvider serviceProvider, ILogger<BaseController<DeviceBrowsers, DeviceBrowserModel, RequestDeviceBrowserModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDeviceBrowserService>();
            this.deviceBrowserService = serviceProvider.GetRequiredService<IDeviceBrowserService>();
        }
        public async override Task<AppDomainResult> AddItem([FromBody] RequestDeviceBrowserModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<DeviceBrowsers>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.deviceBrowserService.InsertOrUpdatePlayerId(item);
                    if (string.IsNullOrEmpty(messageUserCheck))
                    {
                        success = true;
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else throw new Exception(messageUserCheck);    
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }
    }
}

