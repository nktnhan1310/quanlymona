using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestHolidayConfigModel : RequestCoreModel
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
