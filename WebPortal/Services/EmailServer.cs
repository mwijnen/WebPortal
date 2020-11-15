using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailServer : IEmailServer
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var emailPath = @"C:\Users\marce\Desktop\Mails\";
            var emailName = email.Replace("@", "-at-").Replace(".","_") + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".txt";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"email: {email}");
            builder.AppendLine($"subject: {subject}");
            builder.AppendLine($"message: {message}");

            File.WriteAllText(emailPath + emailName, builder.ToString());

            return Task.CompletedTask;
        }
    }
}