using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace QuanLy.Entities.Catalogue
{
    /// <summary>
    /// Danh mục loại dự án
    /// </summary>
    public class ProjectTypes : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Mã màu
        /// </summary>
        [StringLength(100)]
        public string ColorCode { get; set; }

    }
}
