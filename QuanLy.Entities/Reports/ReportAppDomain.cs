using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class ReportAppDomain
    {
        /// <summary>
        /// Số thứ tự báo cáo
        /// </summary>
        public long RowNumber { get; set; }

        /// <summary>
        /// Tổng số item của báo cáo
        /// </summary>
        public int TotalItem { get; set; }
    }
}
