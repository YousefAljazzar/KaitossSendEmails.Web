

using Kaitoss.EmailService;

namespace Kaitoss
{
    public interface IEmailSender
    {
        void SendEmail(Message message, List<string> Emails);
    }
}