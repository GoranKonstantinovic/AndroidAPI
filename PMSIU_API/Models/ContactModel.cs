using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSIU_API.Models
{
    public class ContactModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }       
        public string PhotoURL { get; set; }
    }
}