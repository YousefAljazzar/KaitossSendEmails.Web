using Kaitoss.EmailService;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using MimeKit;


namespace Kaitoss.Implementation
{
    public class EmailSender : IEmailSender, IHostedService
    {
        private readonly EmailConfiguration _emailConfiger;
        private Timer _timer;
        private MimeMessage message1;
        private List<string> _emails;
        public EmailSender(EmailConfiguration emailConfiger)
        {
            _emailConfiger = emailConfiger;
        }

        public void SendEmail(Message message, List<string> Emails)
        {
            var emailMessage = CreateEmailMessage(message);
            message1 = emailMessage;
            _emails = Emails;
            Send(null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Send, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfiger.UserName));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            return emailMessage;
        }

        private async void Send(object state)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiger.SmtpServer, 465, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiger.UserName, _emailConfiger.Password);
                var users = _emails.Chunk(150);
                await Task.WhenAll(users.Select(async a =>
                {
                    foreach (var user in a)
                    {
                        message1.To.Clear();
                        message1.To.Add(new MailboxAddress("Email", user));
                        await client.SendAsync(message1);
                    }
                }));
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}