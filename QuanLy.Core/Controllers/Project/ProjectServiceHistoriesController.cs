using App.Core.Controllers;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
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
    [Route("api/project-service-histories")]
    [ApiController]
    [Authorize]
    [Description("Quản lý lịch sử thông tin dịch vụ của project")]
    public class ProjectServiceHistoriesController : BaseController<ProjectServiceHistories, ProjectServiceHistoriesModel, RequestProjectServiceHistoriesModel, SearchProject>
    {
        protected IServiceTypes serviceTypes;
        protected IProjectServiceHistories projectServiceHistories;
        public ProjectServiceHistoriesController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectServiceHistories, ProjectServiceHistoriesModel, RequestProjectServiceHistoriesModel, SearchProject>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IProjectServiceHistories>();
            this.serviceTypes = serviceProvider.GetRequiredService<IServiceTypes>();
            this.projectServiceHistories = serviceProvider.GetRequiredService<IProjectServiceHistories>();
        }

        /// <summary>
        /// Cập nhật thông tin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestProjectServiceHistoriesModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<ProjectServiceHistories>(itemModel);
            item.Updated = DateTime.UtcNow.AddHours(7);
            item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            var DataServiceType = this.serviceTypes.GetById(item.ServiceTypeId ?? 0);
            if (DataServiceType == null)
            {
                throw new KeyNotFoundException("Item ServiceType không tồn tại");
            }

            var DataProjectService = this.domainService.GetById(item.Id);
            if (DataProjectService == null)
            {
                throw new KeyNotFoundException("Item ID không tồn tại");
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
            success = await this.domainService.UpdateAsync(item);
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetById(int id)
        {

            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.projectServiceHistories.DetailProjectServiceHistories(id);
            if (item != null)
            {
                var itemModel = mapper.Map<ProjectServiceHistoriesModel>(item);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách thông tin theo ProjectServiceID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-list-histories")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetList(int id)
        {

            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.projectServiceHistories.ListProjectServiceHistories(id);
            if (item != null)
            {
                var itemModel = mapper.Map<List<ProjectServiceHistoriesModel>>(item);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }
    }
}
