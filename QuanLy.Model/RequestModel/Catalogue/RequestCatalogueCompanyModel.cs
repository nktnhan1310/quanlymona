using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestCatalogueCompanyModel : RequestCoreCatalogueModel
    {
        /// <summary>
        /// Mã công ty
        /// </summary>
        public int? ParentId { get; set; }
    }
}
