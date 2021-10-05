using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace QuanLy.Entities.Catalogue
{
    /// <summary>
    /// Danh mục công ty
    /// </summary>
    public class Companies : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Mã công ty cha
        /// </summary>
        public int? ParentId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Tên công ty cha
        /// </summary>
        [NotMapped]
        public string ParentCompanyName { get; set; }

        #endregion
    }
}
