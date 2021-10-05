using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class ProjectUsers : AppCoreDomain
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã task
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// Mã việc làm trong ngày
        /// </summary>
        public int? TodoId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

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
        [NotMapped]
        public string ProjectName { get; set; }

        /// <summary>
        /// Tên của user
        /// </summary>
        [NotMapped]
        public string UserFullName { get; set; }

        #endregion

    }
}
