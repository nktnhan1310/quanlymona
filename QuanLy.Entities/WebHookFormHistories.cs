using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    public class WebHookFormHistories : AppCoreDomain
    {
        /// <summary>
        /// Mã thông tin liên hệ
        /// </summary>
        public int? ContactCustomerId { get; set; }

        /// <summary>
        /// Mã khách liên hệ (nếu có)
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tên đầy đủ của khách liên hệ
        /// </summary>
        [StringLength(500)]
        public string UserFullName { get; set; }

        /// <summary>
        /// Mã nguồn
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        /// <summary>
        /// Đường dẫn của form url
        /// </summary>
        public string FormUrl { get; set; }

        /// <summary>
        /// Tên form nguồn
        /// </summary>
        [StringLength(1000)]
        public string CampaignSourceName { get; set; }

        /// <summary>
        /// Mã nguồn
        /// </summary>
        public int? CampaignSourceId { get; set; }
        [StringLength(1000)]
        public string CampaignMediumName { get; set; }

        /// <summary>
        /// Mã 
        /// </summary>
        public int? CampaignMediumId { get; set; }

        /// <summary>
        /// Tên chiến dịch
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Chu ký chiến dịch
        /// </summary>
        public string CampaignTerm { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string CampaignContent { get; set; }
        /// <summary>
        /// Tên người phụ trách
        /// </summary>
        public string PersonChargeName { get; set; }
        /// <summary>
        /// Số điện thoại người phụ trách
        /// </summary>
        public string PersonChargePhone { get; set; }

        /// <summary>
        /// Mã nguồn chiến dịch
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// Link Url từ form trước đó
        /// </summary>
        public string FormPreviousUrl { get; set; }

        /// <summary>
        /// Nguồn từ url
        /// </summary>
        public string FromAnchorUrl { get; set; }

        /// <summary>
        /// Url đích
        /// </summary>
        public string ToDestinationUrl { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên nguồn liên hệ
        /// </summary>
        [NotMapped]
        public string SourceName { get; set; }

        #endregion
    }
}
