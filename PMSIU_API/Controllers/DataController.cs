using Microsoft.AspNet.Identity;
using PMSIU_API.DatabaseModel;
using PMSIU_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMSIU_API.Controllers
{
    [Authorize]
    public class DataController : ApiController
    {
        public IHttpActionResult Get(long? lastSync = null)
        {
            try
            {
                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    DateTime? lastSyncTime = lastSync.HasValue ? new DateTime(lastSync.Value) : (DateTime?) null;
                    string userEmail = User.Identity.GetUserName();
                    string userId = dbContext.AspNetUsers.FirstOrDefault(x => x.Email.Equals(userEmail)).Id;
                    var sentMsgsIds = dbContext.Message.Where(x => lastSync.HasValue ? x.DateTime > lastSyncTime.Value : true).Where(x => x.AccountId.Equals(userId) && x.IsSent).Select(x => x.Id).ToList();
                    var receivedMsgsIds = dbContext.Message.Where(x => lastSync.HasValue ? x.DateTime > lastSyncTime.Value : true).Where(x => !x.AccountId.Equals(userId) && (x.Contact1.Any(z => z.EmailAddress.Equals(userEmail)) || x.Contact2.Any(z => z.EmailAddress.Equals(userEmail)) || x.Contact3.Any(z => z.EmailAddress.Equals(userEmail))) && x.IsSent).Select(x => x.Id).ToList();
                    var draftMsgsIds = dbContext.Message.Where(x => lastSync.HasValue ? x.DateTime > lastSyncTime.Value : true).Where(x => !x.IsSent && x.AccountId.Equals(userId)).Select(x => x.Id).ToList();

                    DataModel retVal = new DataModel();
                    retVal.Sent = GetMessageModelByIds(sentMsgsIds, dbContext).ToList();
                    retVal.Received = GetMessageModelByIds(receivedMsgsIds, dbContext).ToList();
                    retVal.Draft = GetMessageModelByIds(draftMsgsIds, dbContext).ToList();
                    retVal.Contacts = dbContext.Contact.Where(x => lastSync.HasValue ? x.ModificationTime > lastSyncTime.Value : true).Select(z => new ContactModel
                    {
                        Id = z.Id,
                        FirstName = z.FirstName,
                        LastName = z.LastName,
                        EmailAddress = z.EmailAddress,
                        DisplayName = z.DisplayName,
                        PhotoURL = z.PhotoId.HasValue ? z.Photo.Path : null
                    }).ToList();

                    return Ok(retVal);
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        private List<MessageModel> GetMessageModelByIds(List<long> ids, MyDatabaseContext dbContext)
        {
            List<MessageModel> messages = dbContext.Message.Where(x => ids.Contains(x.Id)).Select(x => new MessageModel
            {
                Id = x.Id,
                Subject = x.Subject,
                MessageContent = x.MessageContent,
                Attachments = x.Attachment.Select(z => new AttachmentModel
                {
                    Data = z.Data,
                    Name = z.Name,
                    Type = z.Type
                }).ToList(),
                ContactsBCCIds = x.Contact1.Select(z => z.Id).ToList(),
                ContactsCCIds = x.Contact2.Select(z => z.Id).ToList(),
                ContactsToIds = x.Contact3.Select(z => z.Id).ToList(),
                IsRead = x.IsRead,
                DateTime = x.DateTime,
                FolderId = x.FolderId,
                From = x.AspNetUsers.UserName,
                TagIds = x.Tag.Select(z => z.Id).ToList()


            }).ToList();

            return messages;
        }
    }
}
