using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace QuanLy.Service
{
    public class BaseContactCustomerService<E> : DomainService<E, BaseSearchContactCustomer>, IBaseContactCustomerService<E, BaseSearchContactCustomer>
        where E : BaseContactCustomer
    {
        public BaseContactCustomerService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<PagedList<E>> GetPagedListData(BaseSearchContactCustomer baseSearch)
        {
            PagedList<E> pagedList = new PagedList<E>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = Queryable.Where(GetExpression(baseSearch));
            decimal itemCount = items.Count();
            pagedList = new PagedList<E>()
            {
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };
            return pagedList;
        }

        protected virtual Expression<Func<E, bool>> GetExpression(BaseSearchContactCustomer baseSearch)
        {
            return e => !e.Deleted
            && (!baseSearch.ContactCustomerId.HasValue)
                || (e.ContactCustomerId == baseSearch.ContactCustomerId.Value);
        }

    }
}
