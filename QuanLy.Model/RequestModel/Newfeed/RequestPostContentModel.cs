using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Model.RequestModel.Newfeed
{
    public class RequestPostContentModel : RequestCoreModel
    {
        public string TitlePost { get; set; }
        public string PostIMG { get; set; }
        [DefaultValue(false)]
        public bool BackgroundPost { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        /// 
        [DefaultValue(1)]
        public int PostType { get; set; }
    }
}
