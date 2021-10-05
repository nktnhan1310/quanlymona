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
    [Route("api/task-type")]
    [ApiController]
    [Authorize]
    [Description("Quản lý loại task")]
    public class TaskTypeController : BaseCatalogueController<TaskTypes, TaskTypeModel, RequestTaskTypeModel, BaseSearch>
    {
        public TaskTypeController(IServiceProvider serviceProvider, ILogger<BaseController<TaskTypes, TaskTypeModel, RequestTaskTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ITaskTypeService>();
        }
    }
}
