using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSIU_API.Models
{
    public class MessageModel
    {
        public long Id { get; set; }
        public string UID { get; set; }
        public DateTime DateTime { get; set; }
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        
        public long FolderId { get; set; }

        //public ContactModel From { get; set; }
        //public List<ContactModel> ContactsTo { get; set; }
        //public List<ContactModel> ContactsCC { get; set; }
        //public List<ContactModel> ContactsBCC { get; set; }

        //public List<TagModel> Tags { get; set; }

        public string From { get; set; }
        public List<long> ContactsToIds { get; set; }
        public List<long> ContactsCCIds { get; set; }
        public List<long> ContactsBCCIds { get; set; }

        public List<long> TagIds { get; set; }

        public bool IsRead { get; set; }

        public List<AttachmentModel> Attachments { get; set; }
    }

    public class SendMessageModel
    {
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        public DateTime DateTime { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
        public List<long> ContactsTo { get; set; }
        public List<long> ContactsCC { get; set; }
        public List<long> ContactsBCC { get; set; }
        public bool IsDraft { get; set; }
    }
}