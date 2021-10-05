using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchProject : BaseSearch
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
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Loại dự án
        /// </summary>
        public int? CategoryProjectId { get; set; }
        /// <summary>
        /// Cờ check dự án hoàn thành chưa
        /// </summary>
        [DefaultValue(null)]
        public bool? isDone { get; set; }
        /// <summary>
        /// Search theo user
        /// </summary>
        public List<int> UserIds { get; set; }

    }
}
