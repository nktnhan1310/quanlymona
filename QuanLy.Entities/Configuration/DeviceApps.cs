using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class DeviceApps : AppCoreDomain
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceOS { get; set; }

        /// <summary>
        /// Device token
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string DeviceType { get; set; }

        public string PlayerId { get; set; }

    }
}
