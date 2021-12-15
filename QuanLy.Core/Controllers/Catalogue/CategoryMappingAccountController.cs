using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
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
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Catalogue
{
    [Route("api/catalogue-mapping-account")]
    [ApiController]
    [Description("Quản lý danh mục mapping account với đầu số tổng đài")]
    public class CategoryMappingAccountController : BaseCatalogueController<CategoryMappingAccounts, CategoryMappingAccountModel, RequestCategoryMappingAccountModel, BaseSearch>
    {
        protected IUserInGroupService userInGroupService;
        public CategoryMappingAccountController(IServiceProvider serviceProvider, ILogger<BaseController<CategoryMappingAccounts, CategoryMappingAccountModel, RequestCategoryMappingAccountModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ICategoryMappingAccount>();
            this.userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
        }

        public override async Task<AppDomainResult> AddItem([FromBody] RequestCategoryMappingAccountModel itemModel)
        {                
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<CategoryMappingAccounts>(itemModel);
            item.Created = DateTime.UtcNow.AddHours(7);
            item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            message = await this.catalogueService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(message))
                throw new KeyNotFoundException(message);
            var DataUserInGroup = await this.userInGroupService.GetUserInGroupByUserId(item.UserId, 7);
            if (DataUserInGroup == null)
                throw new KeyNotFoundException("User không là chăm sóc khách hàng");
            success = await this.catalogueService.CreateAsync(item);
            if (success)
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
            return appDomainResult;
        }

        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestCategoryMappingAccountModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            string message = "";
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<CategoryMappingAccounts>(itemModel);
            item.Updated = DateTime.UtcNow.AddHours(7);
            item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            message = await this.catalogueService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(message))
                throw new KeyNotFoundException(message);
            var DataUserInGroup =  await this.userInGroupService.GetUserInGroupByUserId(item.UserId, 7);
            var getData = this.catalogueService.GetById(item.Id);
            if (getData == null)
                throw new KeyNotFoundException("Không có dữ liệu của item");
            else if (DataUserInGroup == null)
                throw new KeyNotFoundException("User không là chăm sóc khách hàng");
            success = await this.catalogueService.UpdateAsync(item);
            if (success)
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
            return appDomainResult;
        }
    }
}
