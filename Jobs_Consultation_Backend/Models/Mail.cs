using System;
using System.Net;
using System.Net.Mail;

namespace Jobs_Consultation_Backend.Models
{
    public class Mail
    {
        // Email configuration
        private string smtpServer = "smtp.gmail.com";
        private int smtpPort = 587;
        private string smtpUsername = "apekama.online@gmail.com";
        private string smtpPassword = "bkwf pmpe lyeg vtiu";
        private bool enableSsl = true;

        // Sender and recipient
        //private string fromEmail = "apekama.online@gmail.com";
        //private string toEmail = "nuwanthikshana@gmail.com";

        public Mail(string toEmail, string body)
        {
            // Create the email message
            MailMessage mailMessage = new MailMessage("apekama.online@gmail.com", toEmail)
            {
                Subject = "Online Consult",
                Body = body
            };

            // Create the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl,
            };

            try
            {
                // Send the email
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Dispose of the resources
                mailMessage.Dispose();
                smtpClient.Dispose();
            }
        }
    }
}
