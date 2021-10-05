using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Models.DomainModel;
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
    [Route("api/source-project-type")]
    [ApiController]
    [Authorize]
    [Description("Quản lý nguồn dự án")]
    public class SourceProjectTypeController : BaseCatalogueController<SourceProjectTypes, SourceProjectTypeModel, RequestCoreCatalogueModel, BaseSearch>
    {
        public SourceProjectTypeController(IServiceProvider serviceProvider, ILogger<BaseController<SourceProjectTypes, SourceProjectTypeModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISourceProjectTypeService>();
        }
    }
}
