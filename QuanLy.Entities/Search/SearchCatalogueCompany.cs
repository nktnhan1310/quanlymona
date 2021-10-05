using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchCatalogueCompany : BaseSearch
    {
        /// <summary>
        /// Mã công ty
        /// </summary>
        public int? CompanyId { get; set; }
        /// <summary>
        /// Mã công ty cha
        /// </summary>
        public int? ParentCompanyId { get; set; }
    }
}
