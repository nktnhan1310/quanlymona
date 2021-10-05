using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class ContactCustomerSaleRequests : BaseContactCustomer
    {

        /// <summary>
        /// Mô tả yêu cầu liên hệ
        /// </summary>
        public string RequestDescription { get; set; }

        /// <summary>
        /// Mô tả yêu cầu liên hệ HTML
        /// </summary>
        public string RequestDescriptionHTML { get; set; }
    }
}
