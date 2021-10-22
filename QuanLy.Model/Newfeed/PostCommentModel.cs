using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Model.Newfeed
{
    public class PostCommentModel: AppCoreDomainModel
    {
        public int PostContentID { get; set; }
        public int PostCommentID { get; set; }
        public int UID { get; set; }
        public string CMT { get; set; }

        [NotMapped]
        public int? SoComment { get; set; }
    }
}
