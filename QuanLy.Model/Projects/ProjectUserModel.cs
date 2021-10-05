using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectUserModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Mã task
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// Mã việc làm trong ngày
        /// </summary>
        public int? TodoId { get; set; }

        /// <summary>
        /// Phần trăm hoa hồng
        /// </summary>
        public double? PercentCommission { get; set; }

        /// <summary>
        /// Loại user
        /// </summary>
        public int? Type { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Tên của user
        /// </summary>
        public string UserFullName { get; set; }

        #endregion
    }
}
