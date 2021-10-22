using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Models.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface.Services.Catalogue;
using QuanLy.Model;
using QuanLy.Model.Catalogue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/service-type")]
    [ApiController]
    [Authorize]
    [Description("Quản lý danh mục")]
    public class ServiceTypeController : BaseCatalogueController<ServiceTypes, ServiceTypeModel, RequestCoreCatalogueModel, BaseSearch>
    {
        public ServiceTypeController(IServiceProvider serviceProvider, ILogger<BaseController<ServiceTypes, ServiceTypeModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IServiceTypes>();
        }
    }
}
