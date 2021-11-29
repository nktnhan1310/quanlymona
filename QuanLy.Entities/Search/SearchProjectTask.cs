using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchProjectTask : BaseSearch
    {
        public int ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }

    }
}
