using MailKit.Net.Smtp;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using MimeKit;
using ShopKart.Extensions;
using ShopKart.Models;
using System;
using System.Linq;

namespace ShopKart.Functions
{
    public static class SendEmail
    {
        [FunctionName("SendEmail")]
        public static void Run([QueueTrigger("emailqueue")]
            QueueItem<EmailMessage> queueItem,
            [Table("settingstable")] IQueryable<ExtendedTableEntity<SmptSettings>> settingsTable,
            TraceWriter log)
        {
            try
            {
                var settingsEntity = settingsTable.Where(x => x.RowKey.Equals("SmtpSettings")).FirstOrDefault();
                if (settingsEntity != null)
                {
                    var settings = settingsEntity.FromTableEntity();

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("ShopKart", queueItem.Value.From));
                    message.To.Add(new MailboxAddress(queueItem.Value.To, queueItem.Value.To));
                    message.Subject = queueItem.Value.Subject;
                    message.Body = new TextPart("html") { Text = queueItem.Value.Body };

                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(settings.Server, settings.Port, settings.IsSecured);
                        if (!string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
                        {
                            client.Authenticate(settings.UserName, settings.Password);
                        }
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                else
                {
                    throw new ApplicationException($"SmtpSettings not found from settingstable.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                throw;
            }
        }
    }
}
