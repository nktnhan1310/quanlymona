﻿using App.Core.Controllers;
using App.Core.Controllers.Auth;
using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Models.AuthModel;
using App.Core.Models.AuthModel.RequestModel;
using App.Core.Utilities;
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
using System.Net;
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
        public class ObjSource
        {
            public int Id { get; set; }
            public string Source { get; set; }
        }

        /// <summary>
        /// Lấy source
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-source")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetSource()
        {
            List<ObjSource> objs = new List<ObjSource>();
            objs.Add(new ObjSource { Id = 0, Source = "Khách hàng tự liên hệ" });
            objs.Add(new ObjSource { Id = 1, Source = "Khách khác giới thiệu" });
            return new AppDomainResult()
            {
                Data = objs,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
