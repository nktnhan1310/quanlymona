using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerMappingRequestModel : RequestCoreContactCustomerModel
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
    }
}
