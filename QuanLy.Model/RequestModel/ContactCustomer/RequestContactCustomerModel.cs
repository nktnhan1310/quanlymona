using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerModel : RequestCoreModel
    {
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [Required]
        [StringLength(20, ErrorMessage = "Số điện thoại phải nhỏ hơn 20 kí tự")]
        public string Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Email phải nhỏ hơn 20 kí tự")]
        public string Email { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên phải nhỏ hơn 500 kí tự")]
        public string FullName { get; set; }

        /// <summary>
        /// Giới tính
        /// 0 => Nữ
        /// 1 => Nam
        /// null => Không xác định
        /// </summary>
        public bool? Gender { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Trạng thái liên hệ
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Trạng thái cho sale
        /// </summary>
        public int SaleStatus { get; set; }

        /// <summary>
        /// Mã công ty
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Tên công ty
        /// </summary>
        [StringLength(1000)]
        public string CompanyName { get; set; }

        /// <summary>
        /// Mã công ty mẹ (nếu có)
        /// </summary>
        public int? ParentCompanyId { get; set; }

        /// <summary>
        /// Mã nhân viên sale
        /// </summary>
        public int? SaleId { get; set; }

        /// <summary>
        /// Mô tả công việc
        /// </summary>
        [StringLength(1000, ErrorMessage = "Mô tả công việc phải nhỏ hơn 1000 kí tự")]
        public string JobDescription { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000, ErrorMessage = "Địa chỉ phải nhỏ hơn 1000 kí tự")]
        public string Address { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Số tiền dự kiến
        /// </summary>
        public double? ExpectedMoney { get; set; }

        /// <summary>
        /// Danh sách danh sách yêu cầu
        /// </summary>
        public IList<RequestContactCustomerMappingRequestModel> ContactCustomerMappingRequests { get; set; }

        /// <summary>
        /// Ghi chú của thông tin liên hệ
        /// </summary>
        public IList<RequestContactCustomerNoteModel> ContactCustomerNotes { get; set; }

        /// <summary>
        /// Danh sách yêu cầu của thông tin liên hệ
        /// </summary>
        public IList<RequestContactCustomerSaleRequestModel> ContactCustomerSaleRequests { get; set; }

        /// <summary>
        /// Danh sách dịch vụ của thông tin liên hệ
        /// </summary>
        public IList<RequestContactCustomerServiceModel> ContactCustomerServices { get; set; }

        /// <summary>
        /// Danh mục file của thông tin liên hệ
        /// </summary>
        public IList<RequestContactCustomerFileModel> ContactCustomerFiles { get; set; }

    }
}
