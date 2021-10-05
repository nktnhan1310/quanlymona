using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities.Catalogue
{
    /// <summary>
    /// Danh mục màu task
    /// </summary>
    public class ColorTasks : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Link image
        /// </summary>
        public string LinkImg { get; set; }
    }
}
