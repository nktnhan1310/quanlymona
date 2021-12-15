using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class ProjectTasks : AppCoreDomain
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã loại task
        /// </summary>
        public int? TaskTypeId { get; set; }

        /// <summary>
        /// Tiêu đề task
        /// </summary>
        [StringLength(500)]
        public string TaskTitle { get; set; }

        /// <summary>
        /// Nội dung task
        /// </summary>
        public string TaskContent { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
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

        [DefaultValue(false)]
        public bool TaskEffect { get; set; }

        /// <summary>
        /// Số ngày làm task
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

        /// <summary>
        /// Tên dự án
        /// </summary>
        [NotMapped]
        public string ProjectName { get; set; }

        /// <summary>
        /// Tên loại task
        /// </summary>
        [NotMapped]
        public string TaskTypeName { get; set; }

        /// <summary>
        /// Danh sách user làm task
        /// </summary>
        [NotMapped]
        public List<int> UserIds { get; set; }

        /// <summary>
        /// Danh sách ToDoList
        /// </summary>
        [NotMapped]
        public List<ProjectToDoList> ProjectToDoLists { get; set; }


        [NotMapped]
        public int? NumberOverDue { get; set; }

        #endregion
    }
}
