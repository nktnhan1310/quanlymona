using App.Core.Utilities;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IReportCoreService<R, T> where R : ReportAppDomain where T : BaseReportSearch
    {
        /// <summary>
        /// Lấy danh sách phân trang báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        Task<PagedList<R>> GetPagedListReport(T baseSearch);

    }
}
