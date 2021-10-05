using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Phiên thanh toán của dự án
    /// </summary>
    public class ProjectSessionPays : AppCoreDomain
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

        #region Extension Properties

        /// <summary>
        /// Tên dự án
        /// </summary>
        [NotMapped]
        public string ProjectName { get; set; }

        #endregion
    }
}
