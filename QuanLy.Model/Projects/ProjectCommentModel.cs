using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectCommentModel : AppCoreDomainModel
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
        /// Nội dung comment
        /// </summary>
        public string CommentContent { get; set; }
    }
}
