using Microsoft.AspNet.Identity;
using PMSIU_API.DatabaseModel;
using PMSIU_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace PMSIU_API.Controllers
{
    [Authorize]
    public class MessageController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    List<MessageModel> retVal = dbContext.Message.Select(x => new MessageModel
                    {
                        Id = x.Id,
                        Subject = x.Subject,
                        MessageContent = x.MessageContent,
                        IsRead = x.IsRead,
                        Attachments = x.Attachment.Select(z => new AttachmentModel
                        {
                            Data = z.Data,
                            Name = z.Name,
                            Type = z.Type
                        }).ToList(),
                        ContactsBCCIds = x.Contact1.Select(z => z.Id).ToList(),
                        ContactsCCIds = x.Contact2.Select(z => z.Id).ToList(),
                        ContactsToIds = x.Contact3.Select(z => z.Id).ToList(),
                        DateTime = x.DateTime,
                        FolderId = x.FolderId,
                        From = x.AspNetUsers.UserName,
                        TagIds = x.Tag.Select(z => z.Id).ToList()                        
                        

                    }).ToList();

                    return Ok(retVal);
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            
        }

        public IHttpActionResult Post(SendMessageModel message)
        {
            try
            {
                

                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    string userEmail = User.Identity.GetUserName();
                    string userId = dbContext.AspNetUsers.FirstOrDefault(x => x.Email.Equals(userEmail)).Id;

                    Message msg = new Message
                    {
                        AccountId = userId,
                        FolderId = 1,
                        MessageContent = message.MessageContent,
                        Subject = message.Subject,
                        DateTime = message.DateTime,
                        IsSent = !message.IsDraft
                    };

                    if (message.Attachments != null)
                    {
                        foreach (var item in message.Attachments)
                        {
                            msg.Attachment.Add(new DatabaseModel.Attachment
                            {
                                Data = item.Data,
                                Name = item.Name,
                                Type = item.Type
                            });
                        }                        
                    }

                    if (message.ContactsTo != null)
                    {
                        List<Contact> contacts = dbContext.Contact.Where(x => message.ContactsTo.Contains(x.Id)).ToList();
                        msg.Contact3 = contacts;
                    }

                    if (message.ContactsCC != null)
                    {
                        List<Contact> contacts = dbContext.Contact.Where(x => message.ContactsCC.Contains(x.Id)).ToList();
                        msg.Contact2 = contacts;
                    }

                    if (message.ContactsBCC != null)
                    {
                        List<Contact> contacts = dbContext.Contact.Where(x => message.ContactsBCC.Contains(x.Id)).ToList();
                        msg.Contact1 = contacts;
                    }

                    AspNetUsers user = dbContext.AspNetUsers.Find(userId);


                    dbContext.Message.Add(msg);
                    dbContext.SaveChanges();
                    if (!message.IsDraft)
                    {
                        string res = SendMessage(msg, user);
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string SendMessage(Message sendMessage, AspNetUsers user)
        {
            try
            {                
                SmtpClient smtp = new SmtpClient();
                smtp.Host = user.Smtp;
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(user.UserName, user.Password);
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(string.Join(";",sendMessage.Contact3.Select(x => x.EmailAddress)));
                if(sendMessage.Contact2.Any())
                    mailMessage.CC.Add(string.Join(";", sendMessage.Contact2.Select(x => x.EmailAddress)));

                if (sendMessage.Contact1.Any())
                    mailMessage.Bcc.Add(string.Join(";", sendMessage.Contact1.Select(x => x.EmailAddress)));
                mailMessage.Subject = sendMessage.Subject;
                mailMessage.Body = sendMessage.MessageContent;
                mailMessage.From = new MailAddress(user.UserName);
                
                smtp.Send(mailMessage);

                return "Succecssfully sent";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public IHttpActionResult Delete(long id)
        {
            try
            {
                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    var msg = dbContext.Message.Find(id);
                    dbContext.Message.Remove(msg);
                    dbContext.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }
        }

        public IHttpActionResult Put(long id)
        {
            try
            {
                using (MyDatabaseContext dbContext = new MyDatabaseContext())
                {
                    var msg = dbContext.Message.Find(id);
                    msg.IsRead = true;
                    dbContext.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }
        }
    }
}
