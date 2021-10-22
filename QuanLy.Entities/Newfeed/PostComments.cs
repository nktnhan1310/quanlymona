using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities.Newfeed
{
    public class PostComments : AppCoreDomain
    {
        public int PostContentID { get; set; }
        public int PostCommentID { get; set; }
        public int UID { get; set; }
        public string CMT { get; set; }

        [NotMapped]
        public int? TotalPage { get; set; }
        [NotMapped]
        public int? SoComment { get; set; }

    }
}
