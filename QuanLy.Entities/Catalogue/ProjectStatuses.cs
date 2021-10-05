using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Entities.Catalogue
{
    public class ProjectStatuses : AppCoreCatalogueDomain
    {
        [StringLength(200)]
        public string StatusColorBg { get; set; }
        [StringLength(200)]
        public string StatusColorText { get; set; }
    }
}
