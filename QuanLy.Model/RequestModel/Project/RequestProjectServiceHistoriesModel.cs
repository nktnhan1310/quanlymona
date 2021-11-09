using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectServiceHistoriesModel : RequestCoreModel
    {
        /// <summary>
        /// ID Dự án dịch vụ
        /// </summary>
        public int? ProjectServiceId { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? StartDate { get; set; }


        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public int? ServiceTypeId { get; set; }

        /// <summary>
        /// Số thời gian
        /// </summary>
        [DefaultValue(1)]
        public int NumberTime { get; set; }

        /// <summary>
        /// Tháng hay năm
        /// </summary>
        [DefaultValue(true)]
        public bool IsMonth { get; set; }
    }
}
