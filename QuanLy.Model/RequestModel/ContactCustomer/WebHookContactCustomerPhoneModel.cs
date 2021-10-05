using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class WebHookContactCustomerPhoneModel
    {
        /// <summary>
        /// Key được cung cấp
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Số gọi vào
        /// </summary>
        public string CallNumber { get; set; }

        /// <summary>
        /// Tên cuộc gọi
        /// </summary>
        public string CallName { get; set; }

        /// <summary>
        /// Số nhận
        /// </summary>
        public string ReceiptNumber { get; set; }

        /// <summary>
        /// Số để tìm kiếm cuộc gọi
        /// </summary>
        public string Key { get; set; }

        public string KeyRinging { get; set; }

        /// <summary>
        /// Trạng thái cuộc gọi
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Hướng gọi
        /// InBound => gọi vào
        /// OutBound => Gọi đi
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Đầu số tổng đài
        /// </summary>
        public string NumberPBX { get; set; }

        /// <summary>
        /// ReceiptNumber - CallNumber – CallName(nếu có trong danh bạ)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Thông tin cuộc gọi
        /// Data chỉ nhận được đối với Popup có trạng thái "Status":"Down
        /// </summary>
        public DataWebHook Data { get; set; }
    }

    public class DataWebHook
    {
        /// <summary>
        /// Tổng thời gian gọi
        /// </summary>
        public string TotalTimeCall { get; set; }

        /// <summary>
        /// Thời gian thực
        /// </summary>
        public string RealTimeCall { get; set; }

        /// <summary>
        /// Link ghi âm cuộc gọi
        /// </summary>
        public string LinkFile { get; set; }

    }
}
