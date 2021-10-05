using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerTawkModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public IList<CustomerContactTawkToRequest> CustomerContactTawkToRequests { get; set; }
    }

    public class CustomerContactTawkToRequest
    {
        public string label { get; set; }
        public string answer { get; set; }
    }

}
