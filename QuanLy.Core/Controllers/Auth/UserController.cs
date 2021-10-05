using App.Core.Controllers;
using App.Core.Controllers.Auth;
using App.Core.Entities;
using App.Core.Models.AuthModel;
using App.Core.Models.AuthModel.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Description("Quản lý người dùng hệ thống")]
    [Authorize]
    public class UserController : UserCoreController
    {
        public UserController(IServiceProvider serviceProvider, ILogger<BaseController<UserCores, UserCoreModel, RequestUserCoreModel, BaseSearchUser>> logger, IConfiguration configuration, IWebHostEnvironment env) : base(serviceProvider, logger, configuration, env)
        {
        }
    }
}
