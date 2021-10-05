using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class TaskTypeModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Mã màu
        /// </summary>
        [StringLength(100)]
        public string ColorCode { get; set; }

        /// <summary>
        /// Thứ tự task
        /// </summary>
        public int Step { get; set; }
    }
}
