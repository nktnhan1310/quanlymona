using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerFileModel : RequestCoreContactCustomerModel
    {
        public string FileName { get; set; }

        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }
    }
}
