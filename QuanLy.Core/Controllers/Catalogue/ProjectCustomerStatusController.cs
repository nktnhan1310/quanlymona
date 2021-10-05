using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Models.DomainModel;
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

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/project-customer-status")]
    [ApiController]
    [Authorize]
    [Description("Quản lý trạng thái khách hàng của dự án")]
    public class ProjectCustomerStatusController : BaseCatalogueController<ProjectCustomerStatuses, ProjectCustomerStatusModel, RequestCoreCatalogueModel, BaseSearch>
    {
        public ProjectCustomerStatusController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectCustomerStatuses, ProjectCustomerStatusModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IProjectCustomerStatusService>();
        }
    }
}
