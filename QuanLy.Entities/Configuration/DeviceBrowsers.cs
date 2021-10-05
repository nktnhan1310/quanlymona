using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class DeviceBrowsers : AppCoreDomain
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        public string PushEndpoint { get; set; }

        public string PushP256DH { get; set; }

        public string PushAuth { get; set; }

    }
}
