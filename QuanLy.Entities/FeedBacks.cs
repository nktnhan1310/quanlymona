using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// FeedBack
    /// </summary>
    public class FeedBacks : AppCoreDomain
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int? UserId { get; set; }

        public int? Q1 { get; set; }
        public int? Q2 { get; set; }
        public int? Q3 { get; set; }
        public int? Q4 { get; set; }
        public int? Q5 { get; set; }
        public int? Source { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

    }
}
