using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchProjectToDoList : BaseSearch
    {
        public int ProjectId { get; set; }

        public int? TaskId { get; set; }
        /// <summary>
        /// Mã người nhận việc
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

    }
}
