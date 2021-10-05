using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class BaseContactCustomerModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã thông tin liên hệ
        /// </summary>
        public int? ContactCustomerId { get; set; }
    }
}
