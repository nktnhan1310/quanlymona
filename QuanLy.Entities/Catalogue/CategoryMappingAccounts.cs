using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class CategoryMappingAccounts : AppCoreCatalogueDomain
    {
        /// <summary>
        /// User chăm sóc khách hàng
        /// </summary>
        public int UserId { get; set; }


        #region Extension Properties
        [NotMapped]
        public string Phone { get; set; }
        [NotMapped]
        public string UserFullName { get; set; }

        #endregion
    }
}
