using Microsoft.AspNet.Identity;
using PMSIU_API.DatabaseModel;
using PMSIU_API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PMSIU_API.Controllers
{
    [Authorize]
    public class ContactController : ApiController
    {
        public IHttpActionResult Get()
        {
            try
            {
                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    List<ContactModel> contacts = dbContext.Contact.Select(z => new ContactModel
                    {
                        Id = z.Id,
                        FirstName = z.FirstName,
                        LastName = z.LastName,
                        EmailAddress = z.EmailAddress,
                        DisplayName = z.DisplayName,
                        PhotoURL = z.PhotoId.HasValue ? z.Photo.Path : "user.png"
                    }).ToList();
                    return Ok(contacts);
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Post(ContactModel contact)
        {
            try
            {
               

                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    string userEmail = User.Identity.GetUserName();
                    string userId = dbContext.AspNetUsers.FirstOrDefault(x => x.Email.Equals(userEmail)).Id;

                    if (dbContext.Contact.Any(x => x.EmailAddress.Equals(contact.EmailAddress)))
                    {
                        return BadRequest("Korisnik sa registrovanom emial adresom vec postoji.");
                    }

                    Contact newContact = new Contact
                    {
                        Id = contact.Id,
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        EmailAddress = contact.EmailAddress,
                        DisplayName = contact.DisplayName,     
                        AccountId = userId,
                        ModificationTime = DateTime.Now
                    };

                    if (!string.IsNullOrEmpty(contact.PhotoURL))
                    {
                        
                        newContact.Photo = new Photo
                        {
                            Path = contact.PhotoURL.Substring(1, contact.PhotoURL.Length - 2)
                        };
                    }

                    dbContext.Contact.Add(newContact);
                    dbContext.SaveChanges();
                    dbContext.Entry<Contact>(newContact).Reload();

                    var contactModel = new ContactModel
                    {
                        Id = newContact.Id,
                        FirstName = newContact.FirstName,
                        LastName = newContact.LastName,
                        EmailAddress = newContact.EmailAddress,
                        DisplayName = newContact.DisplayName,
                        PhotoURL = newContact.PhotoId.HasValue ? newContact.Photo.Path : "user.png"
                    };

                    return Ok(contactModel);
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

      
    }
}
