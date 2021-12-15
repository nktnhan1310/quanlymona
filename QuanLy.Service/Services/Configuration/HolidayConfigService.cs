using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
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
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class HolidayConfigService : DomainService<HolidayConfigs, SearchHolidayConfig>, IHolidayConfigService
    {
        protected readonly IAppDbContext Context;
        public HolidayConfigService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IAppDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_Hodiday_PagingData";
        }

//        protected override SqlParameter[] GetSqlParameters(SearchHolidayConfig baseSearch)
//        {
//            SqlParameter[] parameters =
//{
//                new SqlParameter("@PageIndex", baseSearch.PageIndex),
//                new SqlParameter("@PageSize", baseSearch.PageSize),
//                new SqlParameter("@SearchContent", baseSearch.SearchContent),
//                new SqlParameter("@OrderBy", baseSearch.OrderBy),
//                new SqlParameter("@FromDate", baseSearch.FromDate),
//                new SqlParameter("@ToDate", baseSearch.ToDate),
//            };
//            return parameters;
//        }


//        /// <summary>
//        /// Lấy danh sách phân trang ngày nghỉ
//        /// </summary>
//        /// <param name="baseSearch"></param>
//        /// <returns></returns>
//        public override async Task<PagedList<HolidayConfigs>> GetPagedListData(SearchHolidayConfig baseSearch)
//        {
//            //PagedList<HolidayConfigs> pagedList = new PagedList<HolidayConfigs>();
//            //int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
//            //int take = baseSearch.PageSize;
//            //var items = this.Queryable.Where(e => !e.Deleted && e.Active
//            //&& (!baseSearch.FromDate.HasValue || (e.FromDate <= baseSearch.FromDate))
//            //&& (!baseSearch.ToDate.HasValue || (e.ToDate >= baseSearch.ToDate))
//            //&& (string.IsNullOrEmpty(baseSearch.SearchContent) || (e.Note.ToLower().Contains(baseSearch.SearchContent.ToLower()))            
//            //));
//            //decimal itemCount = items.Count();
//            //pagedList = new PagedList<HolidayConfigs>()
//            //{
//            //    PageIndex = baseSearch.PageIndex,
//            //    PageSize = baseSearch.PageSize,
//            //    TotalItem = (int)itemCount,
//            //    Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync()
//            //};
//            //return pagedList;
//            PagedList<HolidayConfigs> pagedList = new PagedList<HolidayConfigs>();
//            SqlParameter[] parameters = GetSqlParameters(baseSearch);
//            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
//            pagedList.PageIndex = baseSearch.PageIndex;
//            pagedList.PageSize = baseSearch.PageSize;
//            return pagedList;
//        }

//        private async Task<PagedList<HolidayConfigs>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
//        {
//            return await Task.Run(() =>
//            {
//                PagedList<HolidayConfigs> pagedList = new PagedList<HolidayConfigs>();
//                DataTable dataTable = new DataTable();
//                SqlConnection connection = null;
//                SqlCommand command = null;
//                try
//                {
//                    connection = (SqlConnection)Context.Database.GetDbConnection();
//                    command = connection.CreateCommand();
//                    connection.Open();
//                    command.CommandText = commandText;
//                    command.Parameters.AddRange(sqlParameters);
//                    command.CommandType = CommandType.StoredProcedure;
//                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
//                    sqlDataAdapter.Fill(dataTable);
//                    pagedList.Items = MappingDataTable.ConvertToList<HolidayConfigs>(dataTable);
//                    if (pagedList.Items.Any())
//                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
//                    return pagedList;
//                }
//                finally
//                {
//                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
//                        connection.Close();

//                    if (command != null)
//                        command.Dispose();
//                }
//            });
//        }
    }
}
