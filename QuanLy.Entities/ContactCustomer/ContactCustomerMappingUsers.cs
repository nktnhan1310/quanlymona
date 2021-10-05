using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Entities
{
    public class ContactCustomerMappingUsers : AppCoreDomain
    {
        /// <summary>
        /// Mã mapping
        /// </summary>
        [StringLength(50)]
        public string MappingCode { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
    }
}
