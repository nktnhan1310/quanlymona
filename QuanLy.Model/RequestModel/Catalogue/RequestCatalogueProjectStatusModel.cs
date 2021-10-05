using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace QuanLy.Model
{
    public class RequestCatalogueProjectStatusModel : RequestCoreCatalogueModel
    {
        [StringLength(200, ErrorMessage = "Mã màu dưới 200 kí tự")]
        public string StatusColorBg { get; set; }
        [StringLength(200, ErrorMessage = "Tên màu dưới 200 kí tự")]
        public string StatusColorText { get; set; }
    }
}
