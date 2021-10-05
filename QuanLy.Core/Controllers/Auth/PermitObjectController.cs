using App.Core.Controllers;
using App.Core.Controllers.Auth;
using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Models;
using App.Core.Models.AuthModel.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Auth
{
    [Route("api/permit-object")]
    [ApiController]
    [Description("Quản lý chức năng hệ thống")]
    [Authorize]
    public class PermitObjectController : PermitObjectCoreController
    {
        public PermitObjectController(IServiceProvider serviceProvider, ILogger<BaseController<PermitObjectCores, PermitObjectCoreModel, RequestPermitObjectCoreModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }
    }
}
