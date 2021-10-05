using App.Core.Entities.DomainEntity;
using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Liên hệ khách hàng
    /// </summary>
    public class ContactCustomers : AppCoreDomain
    {
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        [StringLength(500)]
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
        [StringLength(1000)]
        public string JobDescription { get; set; }

        /// <summary>
        /// Ngày sale nhận contact
        /// </summary>
        public DateTime? SaleReceiveDate { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
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
        [NotMapped]
        public IList<ContactCustomerMappingRequests> ContactCustomerMappingRequests { get; set; }

        /// <summary>
        /// Ghi chú của thông tin liên hệ
        /// </summary>
        [NotMapped]
        public IList<ContactCustomerNotes> ContactCustomerNotes { get; set; }

        /// <summary>
        /// Danh sách yêu cầu của thông tin liên hệ
        /// </summary>
        [NotMapped]
        public IList<ContactCustomerSaleRequests> ContactCustomerSaleRequests { get; set; }

        /// <summary>
        /// Danh sách dịch vụ của thông tin liên hệ
        /// </summary>
        [NotMapped]
        public IList<ContactCustomerServices> ContactCustomerServices { get; set; }

        /// <summary>
        /// Danh mục file của thông tin liên hệ
        /// </summary>
        [NotMapped]
        public IList<ContactCustomerFiles> ContactCustomerFiles { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên nhân viên sale
        /// </summary>
        [NotMapped]
        public string SaleName { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [NotMapped]
        public string CustomerName { get; set; }

        /// <summary>
        /// Mã tawk chat
        /// </summary>
        [NotMapped]
        public string TawkChatId { get; set; }

        /// <summary>
        /// Tawk Message Id
        /// </summary>
        [NotMapped]
        public string TawkMessagingId { get; set; }

        /// <summary>
        /// Property củi tawk
        /// </summary>
        [NotMapped]
        public string TawkProperty { get; set; }

        /// <summary>
        /// Đường dẫn url từ form
        /// </summary>
        [NotMapped]
        public string FormUrls { get; set; }

        [NotMapped]
        public List<string> ListFormUrl
        {
            get
            {
                var listResult = new List<string>();
                if (!string.IsNullOrEmpty(FormUrls))
                {
                    var listFormUrl = FormUrls.Split(',').ToList();
                    if(listFormUrl != null && listFormUrl.Any())
                    {
                        foreach (var url in listFormUrl)
                        {
                            listResult.Add(AppUtilities.GetPathFromUrl(url));
                        }
                    }
                }
                return listResult;
            }
        }

        /// <summary>
        /// Tên yêu cầu
        /// </summary>
        [NotMapped]
        public string RequestName { get; set; }

        /// <summary>
        /// Tên nguồn
        /// </summary>
        [NotMapped]
        public string SourceName { get; set; }

        /// <summary>
        /// Mã nguồn
        /// </summary>
        [NotMapped]
        public string SourceIds { get; set; }

        [NotMapped]
        public List<int> ListSourceId
        {
            get
            {
                return !string.IsNullOrEmpty(SourceIds) ? SourceIds.Split(';').Select(e => Convert.ToInt32(e)).ToList() : null;
            }
        }

        #endregion
    }
}
