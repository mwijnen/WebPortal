using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IEmailServer
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
