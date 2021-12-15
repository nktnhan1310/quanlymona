using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using Microsoft.Data.SqlClient;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QuanLy.Service
{
    public class ReportSourceService : ReportCoreService<ReportSourceList, SearchReportSource>, IReportSourceService
    {
        public ReportSourceService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper, context)
        {
        }

        /// <summary>
        /// Lấy thông tin store procedure
        /// </summary>
        /// <returns></returns>
        protected override string GetStoreProcName()
        {
            return "Mona_Report_Source_List";
        }

        //protected override SqlParameter[] GetSqlParameters(SearchReportSource baseSearch)
        //{
        //    SqlParameter[] parameters =
        //    {
        //        new SqlParameter("@PageIndex", baseSearch.PageIndex),
        //        new SqlParameter("@PageSize", baseSearch.PageSize),

        //        new SqlParameter("@Month", baseSearch.Month),
        //        new SqlParameter("@Year", baseSearch.Year),
        //        new SqlParameter("@FromDate", baseSearch.FromDate),
        //        new SqlParameter("@ToDate", baseSearch.ToDate),
                
        //        new SqlParameter("@SearchContent", baseSearch.SearchContent),
        //        new SqlParameter("@OrderBy", baseSearch.OrderBy),
        //        new SqlParameter("@TotalPage", SqlDbType.Int, 0),
        //    };
        //    return parameters;
        //}
    }
}
