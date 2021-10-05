using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities.Search
{
    public class SearchContactCustomer : BaseSearch
    {
        /// <summary>
        /// Trạng thái CSKH
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Trạng thái Sale
        /// </summary>
        public int? SaleStatus { get; set; }

        /// <summary>
        /// Danh sách nguồn
        /// </summary>
        public List<int> SourceIds { get; set; }
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Danh sách saler
        /// </summary>
        public List<int> SaleIds { get; set; }
    }
}
