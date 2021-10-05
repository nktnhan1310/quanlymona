using App.Core.Controllers;
using App.Core.Controllers.Auth;
using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Models;
using App.Core.Models.AuthModel;
using App.Core.Utilities;
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
    [Route("api/user-group")]
    [ApiController]
    [Description("Quản lý nhóm người dùng hệ thống")]
    [Authorize]
    public class UserGroupController : UserGroupCoreController
    {
        public UserGroupController(IServiceProvider serviceProvider, ILogger<BaseCatalogueController<UserGroupCores, UserGroupCoreModel, RequestUserGroupCoreModel, BaseSearchUserInGroup>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }

    }
}
