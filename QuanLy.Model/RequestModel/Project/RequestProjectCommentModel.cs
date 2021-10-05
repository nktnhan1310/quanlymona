using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectCommentModel : RequestCoreModel
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        [Required(ErrorMessage = "Mã dự án không hợp lệ")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// Mã task
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// Nội dung comment
        /// </summary>
        public string CommentContent { get; set; }
    }
}
