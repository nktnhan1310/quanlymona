using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class SalaryMonths : AppCoreDomain
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// Tháng
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// Tháng làm
        /// </summary>
        public int? WorkMonth { get; set; }
        /// <summary>
        /// Số ngày nghỉ
        /// </summary>
        public int? DayOff { get; set; }
        /// <summary>
        /// Số ngày OT
        /// </summary>
        public int? OTDays { get; set; }

        /// <summary>
        /// Tổng số ngày làm
        /// </summary>
        public int? TotalWorkDays { get; set; }

        /// <summary>
        /// Lương
        /// </summary>
        public double? SalaryPer { get; set; }
        
        /// <summary>
        /// Lương tháng
        /// </summary>
        public double? SalaryMonth { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }
    }
}
