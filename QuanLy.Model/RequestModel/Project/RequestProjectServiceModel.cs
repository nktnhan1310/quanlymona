using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectServiceModel  : RequestCoreModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public string ServiceTypeCode { get; set; }

        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        public string ServiceTypeName { get; set; }
        /// <summary>
        /// Ngày bắt đầu dịch vụ
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [DefaultValue(1)]
        public int? Status { get; set; }

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

        /// <summary>
        /// Còn sử dụng
        /// </summary>
        [DefaultValue(1)]
        public int? StillInUse { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public int? ServiceTypeId { get; set; }
    }
}
