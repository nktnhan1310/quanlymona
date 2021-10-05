using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Utilities
{
    /// <summary>
    /// Danh mục enum hệ thống
    /// </summary>
    public class CatalogueEnums
    {
        /// <summary>
        /// Trạng thái liên hệ khách hàng của cskh
        /// </summary>
        public enum ContactCustomerStatus
        {
            /// <summary>
            /// Chưa liên hệ
            /// </summary>
            UnConnected = 0,
            /// <summary>
            /// Đang liên hệ
            /// </summary>
            Connecting = 1,
            /// <summary>
            /// Hẹn gọi lại
            /// </summary>
            CallBack = 2,
            /// <summary>
            /// Đã liên hệ
            /// </summary>
            Connected = 3,
            /// <summary>
            /// Sai thông tin
            /// </summary>
            ErrorInformation = 4,
            /// <summary>
            /// Chưa liên hệ được
            /// </summary>
            CantConnect = 5,
            /// <summary>
            /// Chưa có nhu cầu
            /// </summary>
            NoNeed = 6,
        }

        /// <summary>
        /// Trạng thái xử lý của Sale
        /// </summary>
        public enum SaleContactCustomerStatus
        {
            /// <summary>
            /// Chưa liên hệ
            /// </summary>
            UnConnected = 0,
            /// <summary>
            /// Đang deal
            /// </summary>
            Dealing = 1,
            /// <summary>
            /// Đã liên hệ lấy yêu cầu
            /// </summary>
            Connected = 2,
            /// <summary>
            /// Chưa liên hệ được
            /// </summary>
            CantConnect = 3,
            /// <summary>
            /// Deal thành công
            /// </summary>
            DealSuccess = 4

        }

        /// <summary>
        /// Danh sách nguồn yêu cầu đầu vào
        /// </summary>
        public enum ContactCustomerSourceCatalogue
        {
            /// <summary>
            /// Từ form
            /// </summary>
            FORM = 0,
            /// <summary>
            /// Từ thông tin tawk
            /// </summary>
            TAWK = 1,
            /// <summary>
            /// Từ tổng đài
            /// </summary>
            PHONE = 2,
            /// <summary>
            /// Khác
            /// </summary>
            OTHER = 3
        }

        /// <summary>
        /// Phân loại tawk
        /// </summary>
        public enum TawkToType
        {
            /// <summary>
            /// Start chat
            /// </summary>
            Start = 0,
            /// <summary>
            /// End chat
            /// </summary>
            End = 1,
            /// <summary>
            /// Create ticket
            /// </summary>
            Create = 2,
            /// <summary>
            /// Submit prechat form
            /// </summary>
            SubmitPrechat = 3
        }

        /// <summary>
        /// Trạng thái của WebHook
        /// </summary>
        public enum WebHookContactCustomerStatus
        {
            /// <summary>
            /// Đổ chuông (gọi vào)
            /// </summary>
            Ringing = 0,
            /// <summary>
            /// Đổ chuông (gọi ra)
            /// </summary>
            Ringing_Out = 1,

            /// <summary>
            /// Nghe máy (gọi vào)
            /// </summary>
            Up = 2,
            /// <summary>
            /// Nghe máy (gọi ra)
            /// </summary>
            Up_Out = 3,
            /// <summary>
            /// Gác máy (gọi vào)
            /// </summary>
            Down = 4,
            /// <summary>
            /// Gác máy (gọi ra)
            /// </summary>
            Down_Out = 5,
        }

        /// <summary>
        /// Hướng cuộc gọi
        /// </summary>
        public enum WebHookDirectionType
        {
            /// <summary>
            /// Gọi vào
            /// </summary>
            Inbound = 0,
            /// <summary>
            /// Gọi ra
            /// </summary>
            Outbound = 1,
        }

        /// <summary>
        /// Loại file của project
        /// </summary>
        public enum ProjectFileType
        {
            /// <summary>
            /// File ở local
            /// </summary>
            Local = 0,
            /// <summary>
            /// File trên drive
            /// </summary>
            Drive = 1
        }

        /// <summary>
        /// Loại ngày của dự án
        /// </summary>
        public enum DateProjectType
        {
            /// <summary>
            /// Theo ngày
            /// </summary>
            Date = 0,
            /// <summary>
            /// Theo tháng
            /// </summary>
            Month = 1,
            /// <summary>
            /// Theo năm
            /// </summary>
            Year = 2
        }

        /// <summary>
        /// Loại user của project
        /// </summary>
        public enum ProjectUserType
        {
            /// <summary>
            /// Nhân lực dự án
            /// </summary>
            Staff = 0,
            /// <summary>
            /// Khách hàng
            /// </summary>
            User = 1
        }
    }
}
