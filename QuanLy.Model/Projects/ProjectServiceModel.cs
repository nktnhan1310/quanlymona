using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectServiceModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày bắt đầu dịch vụ
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Ngày kết thúc dịch vụ
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        [StringLength(500)]
        public string DeadlineName { get; set; }

        public int? StillInUse { get; set; }
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
