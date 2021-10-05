using App.Core.Controllers.Catalogue;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Model;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/catalogue")]
    [ApiController]
    [Description("Xem danh mục")]
    public class CatalogueController : CatalogueCoreController
    {
        protected ISourceTypeService sourceTypeService;
        protected IRequestTypeService requestTypeService;
        protected ICompanyService companyService;

        #region Projects

        protected IProjectStatusService projectStatusService;
        protected IProjectTypeService projectTypeService;
        protected ISourceProjectTypeService sourceProjectTypeService;
        protected IProjectCustomerStatusService projectCustomerStatusService;

        #endregion


        public CatalogueController(IServiceProvider serviceProvider, IMapper mapper, IConfiguration configuration) : base(serviceProvider, mapper, configuration)
        {
            sourceTypeService = serviceProvider.GetRequiredService<ISourceTypeService>();
            requestTypeService = serviceProvider.GetRequiredService<IRequestTypeService>();
            companyService = serviceProvider.GetRequiredService<ICompanyService>();

            #region Projects

            projectStatusService = serviceProvider.GetRequiredService<IProjectStatusService>();
            projectTypeService = serviceProvider.GetRequiredService<IProjectTypeService>();
            sourceProjectTypeService = serviceProvider.GetRequiredService<ISourceProjectTypeService>();
            projectCustomerStatusService = serviceProvider.GetRequiredService<IProjectCustomerStatusService>();

            #endregion

        }

        #region Projects

        /// <summary>
        /// Lấy danh sách trạng thái của dự án
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-project-status-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ProjectController)]
        public async Task<AppDomainResult> GetProjectStatusCatalogue(string searchContent)
        {
            var pagedList = await this.projectStatusService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower())
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<ProjectStatusModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách trạng thái khách hàng của dự án
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-project-customer-status-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ProjectController)]
        public async Task<AppDomainResult> GetProjectCustomerStatusCatalogue(string searchContent)
        {
            var pagedList = await this.projectCustomerStatusService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower())
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<ProjectCustomerStatusModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách loại hình dự án
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-project-type-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ProjectController)]
        public async Task<AppDomainResult> GetProjectTypeCatalogue(string searchContent)
        {
            var pagedList = await this.projectTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower())
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<ProjectTypeModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách nguồn của dự án
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-project-source-type-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ProjectController)]
        public async Task<AppDomainResult> GetSourceProjectTypeCatalogue(string searchContent)
        {
            var pagedList = await this.sourceProjectTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower())
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<SourceProjectTypeModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion



        /// <summary>
        /// Lấy thông tin danh sách danh mục nguồn
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-source-type-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ContactCustomerController)]
        public async Task<AppDomainResult> GetSourceTypeCatalogue(string searchContent)
        {
            var pagedList = await this.sourceTypeService.GetAsync(e => !e.Deleted && e.Active 
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower()) 
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<SourceTypeModel>>(pagedList);

            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách danh mục yêu cầu
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-request-type-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ContactCustomerController)]
        public async Task<AppDomainResult> GetRequestTypeCatalogue(string searchContent)
        {
            var pagedList = await this.requestTypeService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) || (e.Code.ToLower().Contains(searchContent.ToLower())
            || e.Name.ToLower().Contains(searchContent.ToLower())
            || e.Description.ToLower().Contains(searchContent.ToLower())
            ))
            );
            var pagedListModel = mapper.Map<IList<RequestTypeModel>>(pagedList);

            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách danh mục công ty
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-company-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.CatalogueCompanyController)]
        public async Task<AppDomainResult> GetCompanyCatalogue([FromQuery] SearchCatalogueCompany baseSearch)
        {
            var pagedList = await this.companyService.GetPagedListData(baseSearch);
            var pagedListModel = mapper.Map<PagedList<CompanyModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách trạng thái của cskh
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-cskh-status-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ContactCustomerController)]
        public async Task<AppDomainResult> GetCSKHStatusCatalogue()
        {
            var listStatus = new List<object>()
            {
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                    Name = "Chưa liên hệ"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.Connecting,
                    Name = "Đang liên hệ"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.Connected,
                    Name = "Đã liên hệ"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.CallBack,
                    Name = "Hẹn gọi lại"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.CantConnect,
                    Name = "Chưa liên hệ được"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.ErrorInformation,
                    Name = "Sai thông tin"
                },
                new
                {
                    Id = (int)CatalogueEnums.ContactCustomerStatus.NoNeed,
                    Name = "Chưa có nhu cầu"
                },
            };
            return new AppDomainResult()
            {
                Success = true,
                Data = listStatus,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách trạng thái của sale
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-sale-status-catalogue")]
        [AppAuthorize(new string[] { CoreContants.View }, Contants.ContactCustomerController)]
        public async Task<AppDomainResult> GetSaleStatusCatalogue()
        {
            var listStatus = new List<object>()
            {
                new
                {
                    Id = (int)CatalogueEnums.SaleContactCustomerStatus.UnConnected,
                    Name = "Chưa liên hệ"
                },
                new
                {
                    Id = (int)CatalogueEnums.SaleContactCustomerStatus.Dealing,
                    Name = "Đang deal"
                },
                new
                {
                    Id = (int)CatalogueEnums.SaleContactCustomerStatus.DealSuccess,
                    Name = "Deal thành công"
                },
                new
                {
                    Id = (int)CatalogueEnums.SaleContactCustomerStatus.Connected,
                    Name = "Đã liên hệ lấy yêu cầu"
                },
                new
                {
                    Id = (int)CatalogueEnums.SaleContactCustomerStatus.CantConnect,
                    Name = "Chưa liên hệ được"
                },
            };
            return new AppDomainResult()
            {
                Success = true,
                Data = listStatus,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


    }
}
