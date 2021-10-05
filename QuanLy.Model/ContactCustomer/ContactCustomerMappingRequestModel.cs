using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class ContactCustomerMappingRequestModel : BaseContactCustomerModel
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
        [StringLength(1000, ErrorMessage = "Mô tả phải nhỏ hơn 1000 kí tự")]
        public string Description { get; set; }


        #region Extension Properties

        /// <summary>
        /// Tên yêu cầu
        /// </summary>
        public string RequestName { get; set; }

        /// <summary>
        /// Tên nguồn
        /// </summary>
        public string SourceName { get; set; }

        #endregion
    }
}
