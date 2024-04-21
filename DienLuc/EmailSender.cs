using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace DienLuc
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "your_email";
            var pw = "your_pass";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject: subject,
                                body: message)
            );
        }
    }
}
