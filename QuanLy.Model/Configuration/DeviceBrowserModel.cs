using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model.Configuration
{
    public class DeviceBrowserModel : AppCoreDomainModel
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        public string PushEndpoint { get; set; }

        public string PushP256DH { get; set; }

        public string PushAuth { get; set; }

        public string PlayerId { get; set; }
    }
}
