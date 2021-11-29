using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectToDoListModel : RequestCoreModel
    {
        public int? ProjectId { get; set; }
        /// <summary>
        /// Mã task
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// Mã người nhận việc
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tiêu đề công việc
        /// </summary>
        [StringLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        public DateTime? FromTime { get; set; }

        /// <summary>
        /// Mô tả/ghi chú
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Số giờ làm 
        /// </summary>
        public int NumberHours { get; set; }

    }
}
