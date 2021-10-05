using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestProjectFileModel : RequestCoreFileModel
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
        /// Mã to do list
        /// </summary>
        public int? ToDoListId { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }
    }
}
