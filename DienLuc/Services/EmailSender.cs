using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace DienLuc.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com", //or another email sender provider
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("emphan1975@gmail.com", "nyaktxkvvhpcyelm")
            };

            return client.SendMailAsync("emphan1975@gmail.com", email, subject, message);
        }

    }
}
