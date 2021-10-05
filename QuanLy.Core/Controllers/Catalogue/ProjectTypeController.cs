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
    [Route("api/project-type")]
    [ApiController]
    [Authorize]
    [Description("Quản lý loại dự án")]
    public class ProjectTypeController : BaseCatalogueController<ProjectTypes, ProjectTypeModel, RequestCatalogueProjectTypeModel, BaseSearch>
    {
        public ProjectTypeController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectTypes, ProjectTypeModel, RequestCatalogueProjectTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IProjectTypeService>();
        }
    }
}
