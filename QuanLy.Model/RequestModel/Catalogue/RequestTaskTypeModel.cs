using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Model
{
    public class RequestTaskTypeModel : RequestCoreCatalogueModel
    {
        /// <summary>
        /// Mã màu
        /// </summary>
        [StringLength(100, ErrorMessage = "Mã màu phải nhỏ hơn 100 kí tự")]
        public string ColorCode { get; set; }

        /// <summary>
        /// Thứ tự task
        /// </summary>
        public int Step { get; set; }
    }
}
