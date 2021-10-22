using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities.Newfeed
{
    public class PostContents : AppCoreDomain
    {
        public string TitlePost { get; set; }
        public string PostIMG { get; set; }
        public bool BackgroundPost { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        public int PostType { get; set; }

        [NotMapped]
        public int? TotalPage { get; set; }
        [NotMapped]
        public int? SoComment { get; set; }
    }
}
