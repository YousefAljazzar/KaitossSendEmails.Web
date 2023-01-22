using Kaitoss;
using Kaitoss.DbModels.Models;
using Kaitoss.EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KaitossSendEmails.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly KitoDbContext _dbContext;
        private IEmailSender _emailSender;

        public SendEmailController(KitoDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }
        [Route("SendEmails")]
        [HttpPost]
        public IActionResult SendEmail([FromBody] string Message)
        {
            var usersEmail = _dbContext.Users.Select(a => a.Email).ToList();

            var message = new Message(usersEmail, "Hello",
                "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <style>\r\n    /* Add your CSS styles here */\r\n  " +
                "  body {\r\n      font-family: sans-serif;\r\n color: #333;\r\n    }\r\n    .confirmation-message {\r\n      background-color: #eee;\r\n      padding: 20px;\r\n      border: 1px solid #ccc;\r\n      border-radius: 5px;\r\n    }\r\n    .confirmation-message h1 {\r\n      font-size: 24px;\r\n      margin: 0 0 10px 0;\r\n    }\r\n    .confirmation-message p {\r\n      margin: 0;\r\n      padding: 0;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"confirmation-message\">\r\n    <h1>Massege</h1>\r\n    <p>Dear User,</p>\r\n    <p>Thank you for signing up for our service.</p>" + Message +
                $"\r\n\r\n<p>Best regards,</p>\r\n    <p>The Your-Site Team</p>\r\n  </div>\r\n</body>\r\n</html>");

            _emailSender.SendEmail(message, usersEmail);
            return Ok(new { Message = "Done" });
        }

        [Route("AddUser")]
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return Ok(new { Message = "Done" });
        }
    }
}
