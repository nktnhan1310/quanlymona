using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class ContactCustomerSaleRequestModel : BaseContactCustomerModel
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
