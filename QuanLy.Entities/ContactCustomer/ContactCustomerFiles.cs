using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// File của thông tin liên hệ
    /// </summary>
    public class ContactCustomerFiles : BaseContactCustomerFile
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public int TypeId { get; set; }

        
    }
}
