using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class WebHookDisplayedRequestModel
    {
        public string Content { get; set; }
        public string Event { get; set; }
        public string Heading { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}
