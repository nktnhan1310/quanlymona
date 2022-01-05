using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectSessionPayModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Ngày trả tiền
        /// </summary>
        public DateTime? DatePaid { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Cờ check thanh toán hay chưa?
        /// </summary>
        [DefaultValue(false)]
        public bool IsDone { get; set; }
    }
}
