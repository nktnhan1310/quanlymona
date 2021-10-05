using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuanLy.Entities
{
    public class BaseContactCustomerFile : BaseContactCustomer
    {
        [StringLength(500)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        [StringLength(50)]
        public string FileExtension { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string FileTempName { get; set; }
        public string FileUrl { get; set; }
    }
}
