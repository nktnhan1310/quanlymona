using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLy.Model
{
    public class RequestNotificationSingleModel : RequestCoreModel
    {

        public int NotificationID { get; set; }
        public string NotificationTitle { get; set; }
        public string notifacationContent { get; set; }
        /// <summary>
        /// 0 chưa xem 1 đã xem
        /// </summary>
        [DefaultValue(0)]
        public int Status { get; set; }
        [DefaultValue(1)]
        public int QuickView { get; set; }
        public int TypeNoti { get; set; }
    }
}
