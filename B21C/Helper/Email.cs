using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace B21C.Helper
{
    public class Email
    {
        public static void SendEmail(string ToAddress, string Subject, string Body)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = false;
            client.Host = "mail.bags21century.com";
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = "mail@bags21century.com";
            NetworkCred.Password = "P@ssw0rd";
            client.UseDefaultCredentials = true;
            client.Credentials = NetworkCred;

            MailMessage mail = new MailMessage(new MailAddress("mail@bags21century.com", "Bags21Century"), new MailAddress(ToAddress));
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = Body;

            client.Send(mail);
            client.Dispose();
        }

        public static void SendGrid(string ToAddress, string Subject, string Body)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = false;
            client.Host = "smtp.sendgrid.net";
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = "apikey";
            NetworkCred.Password = "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";
            client.UseDefaultCredentials = true;
            client.Credentials = NetworkCred;

            MailMessage mail = new MailMessage(new MailAddress("mail@bags21century.com", "Bags21Century"), new MailAddress(ToAddress));
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = Body;

            client.Send(mail);
            client.Dispose();
        }

    }
}