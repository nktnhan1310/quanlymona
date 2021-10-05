using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class ReportSourceListModel : ReportAppDomainModel
    {
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày tạo nếu có
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        ///  Tổng số nguồn từ tawk
        /// </summary>
        public int? TotalTawks { get; set; }

        /// <summary>
        ///  Tổng số nguồn từ điện thoại
        /// </summary>
        public int? TotalPhones { get; set; }

        /// <summary>
        ///  Tổng số nguồn từ form
        /// </summary>
        public int? TotalForms { get; set; }

        /// <summary>
        ///  Tổng số nguồn từ khác
        /// </summary>
        public int? TotalOthers { get; set; }

        /// <summary>
        /// Danh sách chi tiết báo cáo theo nguồn
        /// </summary>
        public IList<ReportSourceDetailModel> ReportTotalSourceDetails { get; set; }
    }

    public class ReportSourceDetailModel
    {
        public int? SourceId { get; set; }
        public string SourceCode { get; set; }
        public int? ContactCustomerId { get; set; }
    }

}
