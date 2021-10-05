using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestCatalogueProjectTypeModel : RequestCoreCatalogueModel
    {
        /// <summary>
        /// Mã màu
        /// </summary>
        [StringLength(100, ErrorMessage = "Mã màu phải nhỏ hơn 100 kí tự")]
        public string ColorCode { get; set; }
    }
}
