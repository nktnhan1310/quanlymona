using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class BaseContactCustomer : AppCoreDomain
    {
        /// <summary>
        /// Mã liên hệ khách hàng
        /// </summary>
        public int? ContactCustomerId { get; set; }
    }
}
