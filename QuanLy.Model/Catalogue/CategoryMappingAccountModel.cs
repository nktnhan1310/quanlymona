using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class CategoryMappingAccountModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// User chăm sóc khách hàng
        /// </summary>
        public int UserId { get; set; }


        #region Extension Properties
        public string Phone { get; set; }
        public string UserFullName { get; set; }

        #endregion
    }
}
