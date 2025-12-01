using MarketSentry.Core.Entities;
using MarketSentry.Core.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security; 

namespace MarketSentry.Infrastructure.Services
{
    public class MailKitEmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml, SmtpConfiguration config)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(config.SenderName, config.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    // 1. Ignorar erro de certificado SSL 
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // 2. Lógica inteligente de conexão 
                    SecureSocketOptions socketOptions;

                    if (config.EnableSsl)
                    {
                        // Se for porta 465, usa SSL direto. Se for 587, usa StartTls.
                        socketOptions = config.Port == 465
                            ? SecureSocketOptions.SslOnConnect
                            : SecureSocketOptions.StartTls;
                    }
                    else
                    {
                        // Se SSL estiver desligado
                        socketOptions = SecureSocketOptions.None;
                    }

                    // Conecta usando a opção correta em vez do booleano simples
                    await client.ConnectAsync(config.Host, config.Port, socketOptions);

                    // Autentica apenas se tiver usuário preenchido
                    if (!string.IsNullOrEmpty(config.UserName))
                    {
                        await client.AuthenticateAsync(config.UserName, config.Password);
                    }

                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro crítico ao enviar e-mail: {ex.Message}");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}