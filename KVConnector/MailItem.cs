using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVConnector
{
    public class MailItem
    {
        public string To { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public int Port { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Body { get; set; }
    }
}
