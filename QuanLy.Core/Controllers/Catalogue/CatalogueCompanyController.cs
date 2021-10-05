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
        /// <param name="currentCompanyId"></param>
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
            if(parentCompanyInfos != null && parentCompanyInfos.Any())
                companyModels = mapper.Map<IList<CompanyModel>>(parentCompanyInfos);
            return new AppDomainResult()
            {
                Success = true,
                Data = companyModels,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
