using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class ProjectStatusModel : AppCoreCatalogueDomainModel
    {
        [StringLength(200)]
        public string StatusColorBg { get; set; }
        [StringLength(200)]
        public string StatusColorText { get; set; }

    }
}
