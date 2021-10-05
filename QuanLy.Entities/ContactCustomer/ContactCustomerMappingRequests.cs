using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Bảng mapping yêu cầu của thông tin liên hệ
    /// </summary>
    public class ContactCustomerMappingRequests : BaseContactCustomer
    {
        /// <summary>
        /// Mã yêu cầu
        /// </summary>
        public int? RequestId { get; set; }

        /// <summary>
        /// Mã nguồn
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }


        #region Extension Properties

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

        #endregion

    }
}
