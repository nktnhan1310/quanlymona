using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
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
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Configuration
{
    [Route("api/holiday-config")]
    [ApiController]
    [Description]
    [Authorize]
    public class HolidayConfigController : BaseController<HolidayConfigs, HolidayConfigModel, RequestHolidayConfigModel, SearchHolidayConfig>
    {
        public HolidayConfigController(IServiceProvider serviceProvider, ILogger<BaseController<HolidayConfigs, HolidayConfigModel, RequestHolidayConfigModel, SearchHolidayConfig>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IHolidayConfigService>();
        }
    }
}
