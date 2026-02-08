using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SMTPConfigOptions _smtpConfigOptions;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public EmailSenderService(IOptions<SMTPConfigOptions> smtpConfigOptions, IConfiguration configuration,IWebHostEnvironment environment)
        {
            _smtpConfigOptions = smtpConfigOptions.Value;
            _configuration = configuration;
            _environment = environment;
        }

        private async Task EmailSender(EmailPlaceHolderDTO emailPlaceHolderDTO)
        {
            MailMessage mailMessage = new MailMessage()
            {
                Subject = emailPlaceHolderDTO.Subject,
                From = new MailAddress(_smtpConfigOptions.SenderAddress, _smtpConfigOptions.SenderDisplayName),
                Body = await UpdatePlaceHolderInBody(emailPlaceHolderDTO),
                IsBodyHtml = _smtpConfigOptions.IsBodyHtml
            };

            mailMessage.To.Add(emailPlaceHolderDTO.Email);

            NetworkCredential networkCredential = new NetworkCredential(_smtpConfigOptions.Username, _smtpConfigOptions.Password);

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = _smtpConfigOptions.Host,
                Port = _smtpConfigOptions.Port,
                UseDefaultCredentials = _smtpConfigOptions.UserDefaultCredentials,
                EnableSsl = _smtpConfigOptions.EnableSSL,
                Credentials = networkCredential
            };

            mailMessage.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mailMessage);

        }

        private async Task<string> UpdatePlaceHolderInBody(EmailPlaceHolderDTO emailPlaceHolderDTO)
        {
            if (emailPlaceHolderDTO != null)
            {
                foreach (var placeHolder in emailPlaceHolderDTO.PlaceHolders)
                {
                    if (emailPlaceHolderDTO.Body.Contains(placeHolder.Key))
                    {
                        emailPlaceHolderDTO.Body = emailPlaceHolderDTO.Body.Replace(placeHolder.Key, placeHolder.Value);
                    }
                }
            }
            return emailPlaceHolderDTO.Body;
        }


        private async Task<string> GetEmailTemplate(string templateName)
        {
            var templatePath = Path.Combine(_environment.ContentRootPath, "Template",
        $"{templateName}.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}.html' not found at: {templatePath}");
            }
            return await File.ReadAllTextAsync(templatePath);
        }

        public async Task SendEmailConfirmation(ApplicationUser user, string token)
        {
            var application = _configuration.GetSection("Application");
            var appDomain = application["AppDomain"];
            var ConfirmationLink = application["ConfirmationLink"];

            EmailPlaceHolderDTO emailPlaceHolderDTO = new EmailPlaceHolderDTO()
            {
                Email = user.Email,
                Subject = "Confirmation Email",
                Body = await GetEmailTemplate("EmailTemplate"),
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}",user.UserName),
                    new KeyValuePair<string, string>("{{Email}}",user.Email),
                    new KeyValuePair<string, string>("{{CompanyName}}",_smtpConfigOptions.SenderDisplayName),
                    new KeyValuePair<string, string>("{{ConfirmationLink}}",string.Format(appDomain+ConfirmationLink,user.Id,token))
                }
            };

            await EmailSender(emailPlaceHolderDTO);
        }

        public async Task SendForgotPasswordEmail(ApplicationUser user, string token)
        {
            var application = _configuration.GetSection("Application");
            var appDomain = application["AppDomain"];
            var ForgotPasswordLink = application["ForgotPasswordLink"];

            EmailPlaceHolderDTO emailPlaceHolderDTO = new EmailPlaceHolderDTO()
            {
                Email = user.Email,
                Subject = "Password Reset",
                Body = await GetEmailTemplate("ForgotPasswordEmailTemplate"),
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}",user.UserName),
                    new KeyValuePair<string, string>("{{CompanyName}}",_smtpConfigOptions.SenderDisplayName),
                    new KeyValuePair<string, string>("{{ForgotPasswordLink}}",string.Format(appDomain+ForgotPasswordLink,user.Id,token))
                }
            };

            await EmailSender(emailPlaceHolderDTO);
        }
    }
}
