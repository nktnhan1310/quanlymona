using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Danh mục dịch vụ của liên hệ
    /// </summary>
    public class ContactCustomerServices : BaseContactCustomer
    {
        /// <summary>
        /// Mô tả dịch vụ
        /// </summary>
        public string ServiceDescription { get; set; }
        /// <summary>
        /// Mô tả dịch vụ HTML
        /// </summary>
        public string ServiceDescriptionHTML { get; set; }
    }
}
