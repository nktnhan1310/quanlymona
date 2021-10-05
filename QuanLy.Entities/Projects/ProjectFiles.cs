using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// File của dự án
    /// </summary>
    public class ProjectFiles : AppCoreDomainFile
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
