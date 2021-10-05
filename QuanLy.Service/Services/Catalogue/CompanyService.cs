using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class CompanyService : CatalogueService<Companies, SearchCatalogueCompany>, ICompanyService
    {
        public CompanyService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<PagedList<Companies>> GetPagedListData(SearchCatalogueCompany baseSearch)
        {
            PagedList<Companies> pagedList = new PagedList<Companies>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;
            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.CompanyId.HasValue || e.Id == baseSearch.CompanyId.Value)
            && (!baseSearch.ParentCompanyId.HasValue || e.ParentId == baseSearch.ParentCompanyId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || e.Name.ToLower().Contains(baseSearch.SearchContent.ToLower()))
            );
            decimal itemCount = items.Count();
            pagedList = new PagedList<Companies>()
            {
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync()
            };
            return pagedList;
        }
    }
}
