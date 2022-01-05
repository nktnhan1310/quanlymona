using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
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
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/service-type")]
    [ApiController]
    [Authorize]
    [Description("Quản lý danh mục")]
    public class ServiceTypeController : BaseCatalogueController<ServiceTypes, ServiceTypeModel, RequestCoreCatalogueModel, BaseSearch>
    {
        protected IServiceTypes serviceTypes;
        public ServiceTypeController(IServiceProvider serviceProvider, ILogger<BaseController<ServiceTypes, ServiceTypeModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IServiceTypes>();
            this.serviceTypes = serviceProvider.GetRequiredService<IServiceTypes>();
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool ExistProjectService = await this.serviceTypes.CheckServiceTypeExistProjectService(id);
            if (ExistProjectService)
            {
                throw new Exception("Dịch vụ đã sử dụng không được xóa");
            }

            var DataServiceType = this.catalogueService.GetById(id);
            if(DataServiceType == null)
            {
                throw new Exception("Không có dữ liệu của item");
            }
            else
            {
                string UserPresent = LoginContext.Instance.CurrentUser.UserName;
                if (DataServiceType.CreatedBy != UserPresent)
                {
                    throw new Exception("Lỗi trong quá trình xử lý");
                }
                bool success = await this.catalogueService.DeleteAsync(id);
                if (success)
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    appDomainResult.Success = success;
                }
                else
                {
                    throw new Exception("Lỗi trong quá trình xử lý");
                }
            }   
            return appDomainResult;
        }
    }
}
