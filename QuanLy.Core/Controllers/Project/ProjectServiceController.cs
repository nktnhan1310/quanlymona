using App.Core.Controllers;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using QuanLy.Interface.Services.Catalogue;
using QuanLy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Project
{
    [Route("api/project-service")]
    [ApiController]
    [Authorize]
    [Description("Quản lý thông tin dịch vụ của project")]
    public class ProjectServiceController : BaseController<ProjectServices, ProjectServiceModel, RequestProjectServiceModel, SearchProject>
    {
        protected IServiceTypes serviceTypes;
        protected IProjectService projectService;
        protected IProjectServiceHistories projectServiceHistories;
        public ProjectServiceController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectServices, ProjectServiceModel, RequestProjectServiceModel, SearchProject>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectServiceService>();
            this.serviceTypes = serviceProvider.GetRequiredService<IServiceTypes>();
            this.projectService = serviceProvider.GetRequiredService<IProjectService>();
            this.projectServiceHistories = serviceProvider.GetRequiredService<IProjectServiceHistories>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestProjectServiceModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<ProjectServices>(itemModel);
            item.Created = DateTime.UtcNow.AddHours(7);
            item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            item.Active = true;
            var DataProject = this.projectService.GetById(itemModel.ProjectId ?? 0);
            if (DataProject == null)
            {
                throw new KeyNotFoundException("Item Project không tồn tại");
            }
            if (itemModel.ServiceTypeId != 0)
            {
                var DataServiceType = this.serviceTypes.GetById(itemModel.ServiceTypeId ?? 0);
                if (DataServiceType == null)
                {
                    throw new KeyNotFoundException("Item ServiceType không tồn tại");
                }
            }
            else
            {
                ServiceTypes serviceTypes = new ServiceTypes();
                serviceTypes.Code = itemModel.ServiceTypeCode;
                serviceTypes.Name = itemModel.ServiceTypeName;
                serviceTypes.Created = DateTime.UtcNow.AddHours(7);
                serviceTypes.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                serviceTypes.Active = true;

                var Message = await this.serviceTypes.GetExistItemMessage(serviceTypes);
                if (!string.IsNullOrEmpty(Message))
                {
                    throw new KeyNotFoundException(Message);
                }
                await this.serviceTypes.CreateAsync(serviceTypes);
                item.ServiceTypeId = serviceTypes.Id;
                itemModel.ServiceTypeId = serviceTypes.Id;
            }
            if (itemModel.IsMonth)
            {
                item.EndDate = itemModel.StartDate.Value.AddMonths(itemModel.NumberTime);
                item.DeadlineName = $"{itemModel.NumberTime} tháng";
            }
            else
            {
                item.EndDate = item.StartDate.Value.AddYears(itemModel.NumberTime);
                item.DeadlineName = $"{itemModel.NumberTime} năm";
            }
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            success = await this.domainService.CreateAsync(item);
            if (success)
            {
                var item2 = mapper.Map<ProjectServiceHistories>(itemModel);
                item2.ProjectServiceId = item.Id;
                item2.DeadlineName = item.DeadlineName;
                item2.EndDate = item.EndDate;
                item2.Created = DateTime.UtcNow.AddHours(7);
                item2.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item2.Active = true;
                await this.projectServiceHistories.CreateAsync(item2);
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestProjectServiceModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<ProjectServices>(itemModel);
            item.Updated = DateTime.UtcNow.AddHours(7);
            item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            item.Active = true;
            var DataProjectSerivce = this.domainService.GetById(itemModel.Id);
            if (DataProjectSerivce == null)
            {
                throw new KeyNotFoundException("Item Id không tồn tại");
            }
            if (itemModel.ServiceTypeId != 0)
            {
                var DataServiceType = this.serviceTypes.GetById(itemModel.ServiceTypeId ?? 0);
                if (DataServiceType == null)
                {
                    throw new KeyNotFoundException("Item ServiceType không tồn tại");
                }
            }
            else
            {
                ServiceTypes serviceTypes = new ServiceTypes();
                serviceTypes.Code = itemModel.ServiceTypeCode;
                serviceTypes.Name = itemModel.ServiceTypeName;
                serviceTypes.Created = DateTime.UtcNow.AddHours(7);
                serviceTypes.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                serviceTypes.Active = true;

                var Message = await this.serviceTypes.GetExistItemMessage(serviceTypes);
                if (!string.IsNullOrEmpty(Message))
                {
                    throw new KeyNotFoundException(Message);
                }
                await this.serviceTypes.CreateAsync(serviceTypes);
                item.ServiceTypeId = serviceTypes.Id;
                itemModel.ServiceTypeId = serviceTypes.Id;
            }
            if (itemModel.IsMonth)
            {
                item.EndDate = itemModel.StartDate.Value.AddMonths(itemModel.NumberTime);
                item.DeadlineName = $"{itemModel.NumberTime} tháng";
            }
            else
            {
                item.EndDate = item.StartDate.Value.AddYears(itemModel.NumberTime);
                item.DeadlineName = $"{itemModel.NumberTime} năm";
            }
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            item.ProjectId = DataProjectSerivce.ProjectId;
            success = await this.domainService.UpdateAsync(item);
            if (success)
            {
                var item2 = mapper.Map<ProjectServiceHistories>(itemModel);
                item2.Id = 0;
                item2.ProjectId = DataProjectSerivce.ProjectId;
                item2.ProjectServiceId = item.Id;
                item2.DeadlineName = item.DeadlineName;
                item2.EndDate = item.EndDate;
                item2.Created = DateTime.UtcNow.AddHours(7);
                item2.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item2.Active = true;
                await this.projectServiceHistories.CreateAsync(item2);
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }
        //public async Task<ActionResult> Send()
        //{
        //    AppDomainResult appDomainResult = new AppDomainResult();
        //    //Start
        //    await this.projectService.UpdateServiceExprireDate();
        //    return appDomainResult;
        //    //end
        //}
    }
}
