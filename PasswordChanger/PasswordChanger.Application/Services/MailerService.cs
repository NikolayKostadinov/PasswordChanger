namespace PasswordChanger.Application.Services
{
    using System;
    using System.Configuration;
    using System.Net.Mail;
    using System.Reflection;
    using log4net;
    using Contracts;

    public class MailerService : IMailerService
    {
        private readonly ILog logger;
        private string smtpServer;
        private int port;
        private readonly ISettingsProvider settingsProvider;


        public MailerService(ILog loggerParam, ISettingsProvider settingsProvider)
        {
            this.logger = loggerParam;
            this.settingsProvider = settingsProvider;
            InitializeMailService();
        }

        /// <summary>
        /// Initializes the mail service.
        /// </summary>
        private void InitializeMailService()
        {
            var settings = this.settingsProvider.GetSettings();
            this.To = settings["SMTP_TO"] as string;
            this.From = settings["SMTP_FROM"] as string;
            string smtpServerString = settings["SMTP_SERVER"] as string;
            this.smtpServer = string.IsNullOrEmpty(smtpServerString) ? "192.168.7.195" : smtpServerString;
            this.port = int.Parse(settings["SMTP_PORT"] as string ?? "25");
        }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string SmtpServer
        {
            get
            {
                return this.smtpServer;
            }
        }

        public void SendMail(string message, string title = "Message")
        {
            MailMessage mailMessage = new MailMessage(this.From, this.To, title, message);
            SmtpClient client = new SmtpClient(this.smtpServer, this.port);
            client.UseDefaultCredentials = true;
            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
