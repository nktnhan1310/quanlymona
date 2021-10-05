using App.Core.Controllers;
using App.Core.Controllers.Auth;
using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Models;
using App.Core.Models.DomainModel;
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
    [Route("api/permission")]
    [ApiController]
    [Description("Quản lý quyền hệ thống")]
    [Authorize]
    public class PermissionController : PermissionCoreController
    {
        public PermissionController(IServiceProvider serviceProvider, ILogger<BaseController<PermissionCores, PermissionCoreModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }
    }
}
