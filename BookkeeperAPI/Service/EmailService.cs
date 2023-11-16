namespace BookkeeperAPI.Service
{
    #region usings
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Service.Interface;
    using System.Net;
    using System.Net.Mail;
    #endregion

    public static class EmailService
    {
        public static async Task SendEmail(LoginCredential credentials, MailMessage mailMessage)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Port = 587;
            client.Credentials = new NetworkCredential(credentials.Email, credentials.Password);

            await client.SendMailAsync(mailMessage);
        }
    }
}
