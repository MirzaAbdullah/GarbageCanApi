using GarbageCanApi.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GarbageCanApi.Utilities
{
    public class Email
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<Email> _logger;
        public Email(IConfiguration configuration, ILogger<Email> logger)
        {
            this.configuration = configuration;
            _logger = logger;
        }

        public bool sendEmail(EmailViewModel email)
        {
            if (Convert.ToBoolean(configuration["EmailCredentials:isEmailAllowed"]))
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient();

                    mail.From = new MailAddress(configuration["EmailCredentials:username"].ToString());
                    mail.To.Add(email.to);
                    mail.Subject = string.Format("{0} - GarbageCAN (Pvt.) Ltd", email.subject);

                    mail.IsBodyHtml = true;
                    mail.Body = email.body;

                    SmtpServer.Host = configuration["EmailCredentials:smtpClient"].ToString();
                    SmtpServer.Port = Convert.ToInt32(configuration["EmailCredentials:smtpPort"]);
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(configuration["EmailCredentials:username"].ToString(), configuration["EmailCredentials:password"].ToString());
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sending email.");
                }
            }

            return false;
        }
        public string htmlEmailBody(string verificationBlock)
        {
            string emailBody = string.Empty;
            emailBody += "<html><body><div>";
            emailBody += "<div id='emailHeader' style='width:95%; text-align:center; border: 1px solid #eee; padding:10px; background-color: #eee;'><b>Garbage Can</b></div>";
            emailBody += "<div id='emailBody' style='width:95%; text-align:center; border: 1px solid #eee; padding:10px;'><b> " + verificationBlock + " </b></ div>";
            emailBody += "<div id='emailFooter' style='width:95%; text-align:center; border:1px solid #eee; padding:10px;'>GarbageCAN (Pvt.) Ltd. was established in Karachi in 2017 after carefully studying the inefficiencies, and gaps within the existing system. We started with Karachi due to its obviously lacking waste management facilities, as well as to counter its informal industry, which also leads to other social ills such as child labor, drug addiction, mafia etc. <br /> GarbageCAN aims to make sustainable practices commonplace amongst Pakistanis by taking the buzzword ‘sustainability’ and enabling people to implement it in their daily routine.</div>";
            emailBody += "</div></body></html>";

            return emailBody;
        }
    }
}
