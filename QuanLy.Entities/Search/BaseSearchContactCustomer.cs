using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class BaseSearchContactCustomer : BaseSearch
    {
        /// <summary>
        /// Mã thông tin liên hệ
        /// </summary>
        public int? ContactCustomerId { get; set; }
    }
}
