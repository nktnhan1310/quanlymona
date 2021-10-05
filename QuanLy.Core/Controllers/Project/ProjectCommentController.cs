using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
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

namespace QuanLy.Core.Controllers.Project
{
    [Route("api/project-comment")]
    [ApiController]
    [Authorize]
    [Description("Quản lý thông tin comment của project")]
    public class ProjectCommentController : BaseController<ProjectComments, ProjectCommentModel, RequestProjectCommentModel, BaseSearch>
    {
        public ProjectCommentController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectComments, ProjectCommentModel, RequestProjectCommentModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectCommentService>();
        }
    }
}
