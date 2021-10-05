using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using QuanLy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/project-status")]
    [ApiController]
    [Authorize]
    [Description("Quản lý trạng thái của dự án")]
    public class ProjectStatusController : BaseCatalogueController<ProjectStatuses, ProjectStatusModel, RequestCatalogueProjectStatusModel, BaseSearch>
    {
        public ProjectStatusController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectStatuses, ProjectStatusModel, RequestCatalogueProjectStatusModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IProjectStatusService>();
        }
    }
}
