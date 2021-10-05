using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectModel : RequestCoreModel
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên dự án nhỏ hơn 500 kí tự")]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000, ErrorMessage = "Mô tả nhỏ hơn 1000 kí tự")]
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
        /// Tên đầy đủ của khách
        /// </summary>
        public string CustomerFullName { get; set; }
        /// <summary>
        /// Số điện thoại của khách
        /// </summary>
        public string CustomerPhone { get; set; }
        /// <summary>
        /// Email của khách
        /// </summary>
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Mã khách hàng (nếu có)
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Loại ngày của dự án
        /// 0 => ngày
        /// 1 => tháng
        /// 2 => năm
        /// </summary>
        public int? DateSelectType { get; set; }

        /// <summary>
        /// Nhân lực dự án
        /// </summary>
        public IList<RequestProjectUserModel> ProjectUsers { get; set; }

        /// <summary>
        /// File của dự án
        /// </summary>
        public IList<RequestProjectFileModel> ProjectFiles { get; set; }

        #endregion

    }

}
