using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
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
using System.Net;
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

        public override async Task<AppDomainResult> AddItem([FromBody] RequestCatalogueProjectTypeModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<ProjectTypes>(itemModel);
            item.Created = DateTime.UtcNow.AddHours(7);
            item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            message = await this.catalogueService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(message))
                throw new KeyNotFoundException(message);
            success = await this.catalogueService.CreateAsync(item);
            if (success)
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
            return appDomainResult;
        }

        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestCatalogueProjectTypeModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<ProjectTypes>(itemModel);
            item.Updated = DateTime.UtcNow.AddHours(7);
            item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            message = await this.catalogueService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(message))
                throw new KeyNotFoundException(message);
            var getData = this.catalogueService.GetById(item.Id);
            if (getData == null)
                throw new KeyNotFoundException("Không có dữ liệu của item");
            success = await this.catalogueService.UpdateAsync(item);
            if (success)
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
            return appDomainResult;
        }
    }
}
