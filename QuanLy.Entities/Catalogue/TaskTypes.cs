using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace QuanLy.Entities.Catalogue
{
    /// <summary>
    /// Loại task
    /// </summary>
    public class TaskTypes : AppCoreCatalogueDomain
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
