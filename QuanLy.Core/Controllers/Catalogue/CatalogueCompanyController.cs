using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
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
    [Route("api/catalogue-company")]
    [ApiController]
    [Description("Quản lý danh mục công ty")]
    public class CatalogueCompanyController : BaseCatalogueController<Companies, CompanyModel, RequestCatalogueCompanyModel, SearchCatalogueCompany>
    {
        public CatalogueCompanyController(IServiceProvider serviceProvider, ILogger<BaseController<Companies, CompanyModel, RequestCatalogueCompanyModel, SearchCatalogueCompany>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ICompanyService>();
        }

        /// <summary>
        /// Lấy thông tin công ty cha từ thông tin công ty con hiện tại
        /// </summary>
        /// <param name="currentCompanyId"></param>s
        /// <returns></returns>
        [HttpGet("get-parent-company")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetParentCompany([FromQuery] int? currentCompanyId)
        {

            IList<CompanyModel> companyModels = new List<CompanyModel>();
            var parentCompanyInfos = await this.catalogueService.GetAsync(e => !e.Deleted && e.Active
            && e.ParentId.HasValue && e.ParentId.Value > 0
            && (!currentCompanyId.HasValue || currentCompanyId.Value <= 0 || e.Id != currentCompanyId.Value)
            );
            if (parentCompanyInfos != null && parentCompanyInfos.Any())
                companyModels = mapper.Map<IList<CompanyModel>>(parentCompanyInfos);
            return new AppDomainResult()
            {
                Success = true,
                Data = companyModels,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


        /// <summary>
        /// Lấy danh sách thông tin công ty cha 
        /// </summary>
        /// <param></param>s
        /// <returns></returns>
        [HttpGet("get-list-parent-company")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetListParentCompany()
        {
            IList<CompanyModel> companyModels = new List<CompanyModel>();
            var parentCompanyInfos = await this.catalogueService.GetAsync(e => !e.Deleted && e.Active);
            if (parentCompanyInfos != null && parentCompanyInfos.Any())
                companyModels = mapper.Map<IList<CompanyModel>>(parentCompanyInfos);
            return new AppDomainResult()
            {
                Success = true,
                Data = companyModels,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
        /// <summary>
        /// thêm công ty
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        public override async Task<AppDomainResult> AddItem([FromBody] RequestCatalogueCompanyModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<Companies>(itemModel);
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

        /// <summary>
        /// cập nhật thông tin
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestCatalogueCompanyModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<Companies>(itemModel);
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

        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = await this.catalogueService.DeleteAsync(id);
            if (success)
            {
                var getListData = await this.catalogueService.GetAsync(x => x.ParentId == id);
                foreach(var item in getListData)
                {
                    await this.catalogueService.DeleteAsync(item.Id);
                }
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");
            return appDomainResult;
        }
    }
}
