using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using MongoDB.Driver.Linq;

namespace HomeAutomation.DataAccess {
    public class EmailManager {
        public static void SendEmail(String to, String subject, String messagetext) {
            
            var fromEmail = DatabaseFacade.DatabaseManager.Configurations.AsQueryable().SingleOrDefault(u => u.Name == "FromEmailAdress");
            var server = DatabaseFacade.DatabaseManager.Configurations.AsQueryable().SingleOrDefault(u => u.Name == "MailServer");
            if (fromEmail != null & server != null) {
                MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(to);
                message.Subject = subject;
                message.From = new System.Net.Mail.MailAddress(fromEmail.Value);
                message.Body = messagetext;
                SmtpClient smtp = new System.Net.Mail.SmtpClient(server.Value);
                smtp.Send(message);
            }
        }
    }
}
