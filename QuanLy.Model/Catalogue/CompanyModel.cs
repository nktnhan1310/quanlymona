using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class CompanyModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Mã công ty cha
        /// </summary>
        public int? ParentId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Tên công ty cha
        /// </summary>
        public string ParentCompanyName { get; set; }

        #endregion
    }
}
