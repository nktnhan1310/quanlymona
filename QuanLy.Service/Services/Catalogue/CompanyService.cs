using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class CompanyService : CatalogueService<Companies, SearchCatalogueCompany>, ICompanyService
    {
        protected readonly IAppDbContext Context;
        public CompanyService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IAppDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
            this.IsUseStore = true;
        }

        public override async Task<string> GetExistItemMessage(Companies item)
        {
            var message = "";
            var isExists = await this.unitOfWork.Repository<Companies>().GetQueryable()
                .Where(x =>
                x.Id != item.Id &&
                x.Code == item.Code)
                .AnyAsync();
            if (isExists)
            {
                message = "Mã công ty đã tồn tại";
            }
            return message;
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_load_Companies_PagingData";
        }
    }
}
