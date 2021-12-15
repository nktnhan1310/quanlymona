using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public double Day { get
            {
                return (this.ToDate.Value - this.FromDate.Value).TotalDays + 1;
            } }

        #region Extension Properties
        [NotMapped]
        public int? TotalPage { get; set; }

        #endregion
    }
}
