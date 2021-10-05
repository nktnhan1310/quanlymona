using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Lịch sử web hook
    /// </summary>
    public class WebHookPhoneHistories : AppCoreDomain
    {
        /// <summary>
        /// Khóa API (nếu có)
        /// </summary>
        [StringLength(200)]
        public string ApiKey { get; set; }

        /// <summary>
        /// Số gọi
        /// </summary>
        [StringLength(50)]
        public string CallNumber { get; set; }

        /// <summary>
        /// Tên người gọi
        /// </summary>
        [StringLength(100)]
        public string CallName { get; set; }

        /// <summary>
        /// Đầu số nhận
        /// </summary>
        [StringLength(20)]
        public string ReceiptNumber { get; set; }


        [StringLength(100)]
        public string Key { get; set; }

        [StringLength(100)]
        public string KeyRinging { get; set; }

        [StringLength(200)]
        public string NumberPBX { get; set; }

        /// <summary>
        /// Tin nhắn (nếu có)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Trạng thái cuộc gọi
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>
        /// Tổng thời gian cuộc gọi
        /// </summary>
        [StringLength(50)]
        public string TotalTimeCall { get; set; }

        /// <summary>
        /// Link cuộc gọi
        /// </summary>
        [StringLength(2000)]
        public string LinkFile { get; set; }

        /// <summary>
        /// Nguồn
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// Mã User nếu có
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Thời gian gọi thực tế
        /// </summary>
        public string RealTimeCall { get; set; }

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Mã thông tin liên hệ nếu có
        /// </summary>
        public int? ContactCustomerId { get; set; }

        /// <summary>
        /// Hướng cuộc gọi
        /// </summary>
        [StringLength(100)]
        public string Direction { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên nguồn
        /// </summary>
        [NotMapped]
        public string SourceName { get; set; }

        #endregion

    }
}
