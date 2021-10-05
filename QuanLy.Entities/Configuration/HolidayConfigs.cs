using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Cấu hình ngày nghỉ/ngày lễ
    /// </summary>
    public class HolidayConfigs : AppCoreDomain
    {
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
