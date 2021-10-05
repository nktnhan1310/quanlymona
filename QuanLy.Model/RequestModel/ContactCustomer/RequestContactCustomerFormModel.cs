using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerFormModel
    {
        /// <summary>
        /// mã nguồn web
        /// </summary>
        public string utm_campagin_id { get; set; }

        /// <summary>
        /// Tên nguồn chi tiết
        /// </summary>
        public string utm_campagin_source { get; set; }
        /// <summary>
        /// Tên loại chiến dịch
        /// </summary>
        public string utm_campagin_medium { get; set; }

        /// <summary>
        /// Tên chiến dịch
        /// </summary>
        public string utm_campaign_name { get; set; }

        /// <summary>
        /// Tên nhóm quảng cáo
        /// </summary>
        public string utm_campaign_term { get; set; }

        /// <summary>
        /// Tên post quảng cáo
        /// </summary>
        public string utm_campaign_content { get; set; }

        /// <summary>
        /// Người phụ trách
        /// </summary>
        public string utm_nguoi_phu_trach { get; set; }

        /// <summary>
        /// SDT người phụ trạch
        /// </summary>
        public string utm_phone_nguoi_phu_trach { get; set; }


        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string full_name { get; set; }
        /// <summary>
        /// Yêu cầu
        /// </summary>
        public string request { get; set; }
        /// <summary>
        /// Mô tả yêu cầu
        /// </summary>
        public string request_description { get; set; }

        /// <summary>
        /// url nơi đặt cái link CTA
        /// </summary>
        public string from_anchor { get; set; }

        /// <summary>
        /// url CTA
        /// </summary>
        public string to_destination { get; set; }

        /// <summary>
        /// Url Nguồn giới thiệu
        /// </summary>
        public string form_url { get; set; }

        /// <summary>
        /// Url của trang trước
        /// </summary>
        public string previous_form_url { get; set; }
    }
}
