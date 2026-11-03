using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CinemaManagementSystem.Service
{
    public class GmailService
    {
        private static readonly string[] Scopes =
        {
            Google.Apis.Gmail.v1.GmailService.Scope.GmailSend
        };

        public static async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string body)
        {
            string credentialPath = Path.Combine(
                AppContext.BaseDirectory,
                "Credentials",
                "credentials.json");

            string tokenPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "CinemaManagementSystem",
                "GoogleToken");

            using (var stream = new FileStream(
                credentialPath,
                FileMode.Open,
                FileAccess.Read))
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true));

                var service = new Google.Apis.Gmail.v1.GmailService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Cinema Management System"
                    });

                string message =
                    $"To: {recipientEmail}\r\n" +
                    $"Subject: {subject}\r\n" +
                    "Content-Type: text/plain; charset=utf-8\r\n" +
                    "MIME-Version: 1.0\r\n\r\n" +
                    body;

                var email = new Message
                {
                    Raw = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(message))
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .Replace("=", "")
                };

                await service.Users.Messages.Send(email, "me").ExecuteAsync();
            }
        }
    }
}
