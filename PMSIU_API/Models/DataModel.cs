using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSIU_API.Models
{
    public class DataModel
    {
        public List<MessageModel> Sent { get; set; }
        public List<MessageModel> Received { get; set; }
        public List<MessageModel> Draft { get; set; }
        public List<ContactModel> Contacts { get; set; }
    }
}