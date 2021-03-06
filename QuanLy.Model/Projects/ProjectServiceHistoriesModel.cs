using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectServiceHistoriesModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int? ProjectServiceId { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// hạn dùng
        /// </summary>
        public string DeadlineName { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public int? ServiceTypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên dự án
        /// </summary>
        [NotMapped]
        public string ProjectName { get; set; }

        /// <summary>
        /// Tên dịch vụ sử dụng
        /// </summary>
        [NotMapped]
        public string ProjectServiceName { get; set; }

        #endregion
    }
}
