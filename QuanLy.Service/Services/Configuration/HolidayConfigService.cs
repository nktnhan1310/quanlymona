using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class HolidayConfigService : DomainService<HolidayConfigs, SearchHolidayConfig>, IHolidayConfigService
    {
        public HolidayConfigService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Lấy danh sách phân trang ngày nghỉ
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<HolidayConfigs>> GetPagedListData(SearchHolidayConfig baseSearch)
        {
            PagedList<HolidayConfigs> pagedList = new PagedList<HolidayConfigs>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;
            var items = this.Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.FromDate.HasValue || (e.FromDate <= baseSearch.FromDate))
            && (!baseSearch.ToDate.HasValue || (e.ToDate >= baseSearch.ToDate))
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || (e.Note.ToLower().Contains(baseSearch.SearchContent.ToLower()))            
            ));
            decimal itemCount = items.Count();
            pagedList = new PagedList<HolidayConfigs>()
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
