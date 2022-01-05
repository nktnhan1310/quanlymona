using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class SearchProjectComment : BaseSearch
    {
        public int ProjectId { get; set; }
    }
}
