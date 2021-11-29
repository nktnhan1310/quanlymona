using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Models.DomainModel;
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
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Configuration
{
    [Route("api/device-app")]
    [ApiController]
    [Authorize]
    public class DeviceAppController : BaseController<DeviceApps, DeviceAppModel, RequestDeviceAppModel, BaseSearch>
    {
        public DeviceAppController(IServiceProvider serviceProvider, ILogger<BaseController<DeviceApps, DeviceAppModel, RequestDeviceAppModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDeviceAppService>();
        }
    }
}
