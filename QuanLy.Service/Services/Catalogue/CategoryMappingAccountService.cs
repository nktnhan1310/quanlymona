using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class CategoryMappingAccountService : CatalogueService<CategoryMappingAccounts, BaseSearch>, ICategoryMappingAccount
    {
        protected readonly IAppDbContext Context;
        public CategoryMappingAccountService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IAppDbContext Context) : base(unitOfWork, mapper)
        {
            this.IsUseStore = true;
            this.Context = Context;
        }

        public override async Task<string> GetExistItemMessage(CategoryMappingAccounts item)
        {
            var message = "";
            var isExists = await this.unitOfWork.Repository<CategoryMappingAccounts>().GetQueryable()
                .Where(x =>
                x.Id != item.Id &&
                x.Code == item.Code)
                .AnyAsync();
            if (isExists)
            {
                message = "Mã mapping đã tồn tại";
            }
            return message;  
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_load_Category_Mapping_Account_PagingData";
        }
    }
}
