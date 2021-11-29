using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class NotificationSingles : AppCoreDomain
    {
        public int NotificationID { get; set; }
        public string NotificationTitle { get; set; }
        public string notifacationContent { get; set; }
        public int UID { get; set; }
        /// <summary>
        /// 0 chưa xem 1 đã xem
        /// </summary>
        public int Status { get; set; }
        public int QuickView { get; set; }
        public int TypeNoti { get; set; }

        [NotMapped]
        public int ? TotalPage { get; set; }

    }
}
