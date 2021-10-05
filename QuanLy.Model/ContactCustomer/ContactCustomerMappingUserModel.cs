using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class ContactCustomerMappingUserModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã mapping
        /// </summary>
        [StringLength(50, ErrorMessage = "Mã phải nhỏ hơn 50 kí tự")]
        public string MappingCode { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
    }
}
