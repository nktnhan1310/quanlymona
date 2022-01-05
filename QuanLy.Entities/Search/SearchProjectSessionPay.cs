using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchProjectSessionPay : BaseSearch
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Cờ check thanh toán hay chưa?
        /// </summary>
        [DefaultValue(false)]
        public bool IsDone { get; set; }
    }
}
