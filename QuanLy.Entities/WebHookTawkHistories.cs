using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class WebHookTawkHistories : AppCoreDomain
    {
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [StringLength(1000)]
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tên đầy đủ của người liên hệ (nếu có)
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Mã thông tin liên hệ
        /// </summary>
        public int? ContactCustomerId { get; set; }

        /// <summary>
        /// Mã nguồn
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// Mã tawk message Id
        /// </summary>
        [StringLength(100)]
        public string TawkMessagingId { get; set; }

        /// <summary>
        /// Mã tawk chat
        /// </summary>
        [StringLength(100)]
        public string TawkChatId { get; set; }

        /// <summary>
        /// Property của tawk
        /// </summary>
        public string TawkProperty { get; set; }

        /// <summary>
        /// Loại tawk
        /// </summary>
        public int? TawkType { get; set; }

        /// <summary>
        /// Mã vé
        /// </summary>
        [StringLength(100)]
        public string TawkTicketId { get; set; }

        /// <summary>
        /// Link Url trang liên hệ của tawk
        /// </summary>
        [StringLength(2000)]
        public string TawkUrl { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên nguồn liên hệ
        /// </summary>
        [NotMapped]
        public string SourceName { get; set; }

        #endregion
    }
}
