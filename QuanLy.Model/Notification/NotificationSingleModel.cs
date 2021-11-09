using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class NotificationSingleModel : AppCoreDomainModel
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
    }
}
