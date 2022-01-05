using App.Core.Controllers;
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
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Project
{
    [Route("api/project-session-pay")]
    [ApiController]
    [Authorize]
    [Description("Quản lý các phiên thanh toán của project")]
    public class ProjectSessionPayController : BaseController<ProjectSessionPays, ProjectSessionPayModel, RequestProjectSessionPayModel, SearchProjectSessionPay>
    {
        public ProjectSessionPayController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectSessionPays, ProjectSessionPayModel, RequestProjectSessionPayModel, SearchProjectSessionPay>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectSessionPayService>();
        }
    }
}
