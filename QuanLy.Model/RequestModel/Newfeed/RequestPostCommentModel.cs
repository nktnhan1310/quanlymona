using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model.RequestModel.Newfeed
{
    public class RequestPostCommentModel : RequestCoreModel
    {
        public int PostContentID { get; set; }
        public int? PostCommentID { get; set; }
        public string CMT { get; set; }
    }
}
