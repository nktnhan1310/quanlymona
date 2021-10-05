using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class WebHookTawkRequestModel
    {
        public string Event { get; set; }
        public string chatId { get; set; }
        public string time { get; set; }

        public WebHookTawkMessageData message { get; set; }

        public WebHookTawkVisitor visitor { get; set; }


        public WebHookTawkProperty property { get; set; }

        public WebHookTawkRequester requester { get; set; }
        public WebHookTawkTicket ticket { get; set; }
    }

    public class WebHookTawkMessageData
    {
        public string text { get; set; }
        public string type { get; set; }
        public WebHookSenderObject sender { get; set; }
    }

    public class WebHookSenderObject
    {
        public string type { get; set; }
    }

    public class WebHookTawkVisitor
    {
        public string name { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }

    public class WebHookTawkProperty
    {
        public string id { get; set; }
        public string name { get; set; }

    }

    public class WebHookTawkRequester
    {
        public string name { get; set; }
        public string email { get; set; }
        public string type { get; set; }
    }

    public class WebHookTawkTicket
    {
        public string id { get; set; }
        public int? humanId { get; set; }
        public string subject { get; set; }
        public string message { get; set; }

    }
}
