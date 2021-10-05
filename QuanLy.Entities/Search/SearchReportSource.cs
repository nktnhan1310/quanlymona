using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchReportSource : BaseReportSearch
    {
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// Tháng
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// Năm
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// Loại tìm kiếm
        /// 0 => theo ngày
        /// 1 => theo tháng/năm
        /// 2 => theo năm
        /// </summary>
        public int SearchType { get; set; }
    }
}
