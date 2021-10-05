using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerServiceModel : RequestCoreContactCustomerModel
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
