using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities.Catalogue
{
    /// <summary>
    /// Danh mục dịch vụ
    /// </summary>
    public class ServiceTypes : AppCoreCatalogueDomain
    {
        [NotMapped]
        public int? TotalPage { get; set; }

    }
}
