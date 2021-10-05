using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Dự án
    /// </summary>
    public class Projects : AppCoreDomain
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Số ngày dự kiến
        /// </summary>
        public int? QuantityTime { get; set; }

        /// <summary>
        /// Loại ngày
        /// </summary>
        public int? DateTypeId { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// NGày kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Giá trị dự án
        /// </summary>
        public double? ExpenseProject { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? StatusId { get; set; }

        /// <summary>
        /// Trạng thái khách hàng
        /// </summary>
        public int? CustomerStatusId { get; set; }

        [DefaultValue(false)]
        public bool SaleProject { get; set; }

        /// <summary>
        /// Nguồn dự án
        /// </summary>
        public int? SourceProjectId { get; set; }

        /// <summary>
        /// Loại dự án
        /// </summary>
        public int? CategoryProjectId { get; set; }

        /// <summary>
        /// Số ngày còn lại
        /// </summary>
        public int? LeftDayDeadline { get; set; }

        /// <summary>
        /// Mã dự án cha
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã người quản lý dự án
        /// </summary>
        public int? ProjectManagerId { get; set; }

        /// <summary>
        /// Mã sale
        /// </summary>
        public int? SaleManagerId { get; set; }

        public DateTime? HandOverDate { get; set; }

        /// <summary>
        /// Mã googledrive
        /// </summary>
        public string GoogleDriveId { get; set; }

        /// <summary>
        /// Đường link zalo
        /// </summary>
        public string LinkZalo { get; set; }

        /// <summary>
        /// Ngày deadline
        /// </summary>
        public DateTime? DeadlineDate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        [NotMapped]
        public string QuantityTimeDisplay { get; set; }

        /// <summary>
        /// Nhân lực dự án
        /// </summary>
        [NotMapped]
        public IList<ProjectUsers> ProjectUsers { get; set; }

        /// <summary>
        /// File của dự án
        /// </summary>
        [NotMapped]
        public IList<ProjectFiles> ProjectFiles { get; set; }

        /// <summary>
        /// Phiên thanh toán của dự án
        /// </summary>
        [NotMapped]
        public IList<ProjectSessionPays> ProjectSessionPays { get; set; }

        #endregion
    }
}
