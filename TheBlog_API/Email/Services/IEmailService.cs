using TheBlog_API.Email.Models;

namespace TheBlog_API.Email.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
