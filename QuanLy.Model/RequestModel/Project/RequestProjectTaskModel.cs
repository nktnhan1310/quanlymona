using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectTaskModel : RequestCoreModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        [Required(ErrorMessage = "Mã dự án không hợp lệ")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã loại task
        /// </summary>
        public int? TaskTypeId { get; set; }

        /// <summary>
        /// Tiêu đề task
        /// </summary>
        [StringLength(500, ErrorMessage = "Tiêu đề task không được vượt qua 500 kí tự")]
        public string TaskTitle { get; set; }

        /// <summary>
        /// Nội dung task
        /// </summary>
        public string TaskContent { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime? TimeDone { get; set; }

        /// <summary>
        /// Cờ check Task hoàn thành chưa?
        /// </summary>
        [DefaultValue(false)]
        public bool IsDone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? StatusDay { get; set; }

        /// <summary>
        /// Mã task cha (nếu có)
        /// </summary>
        public int? ParentTaskId { get; set; }

        /// <summary>
        /// Ảnh hưởng của task
        /// </summary>
        [DefaultValue(false)]
        public bool TaskEffect { get; set; }

        /// <summary>
        /// Số ngày hiệu quả
        /// </summary>
        public int WorkDayEffect { get; set; }

        /// <summary>
        /// Thứ tự task
        /// </summary>
        public int TaskIndex { get; set; }

        /// <summary>
        /// Màu của task
        /// </summary>
        public int? ColorTaskId { get; set; }

        [DefaultValue(false)]
        public bool TaskInfinity { get; set; }

        #region Extension Properties

        public List<int> UserIds { get; set; }

        #endregion

    }
}
