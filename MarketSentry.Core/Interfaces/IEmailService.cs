using MarketSentry.Core.Entities;

namespace MarketSentry.Core.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia um e-mail assíncrono.
        /// </summary>
        /// <param name="toEmail">Destinatário</param>
        /// <param name="subject">Assunto</param>
        /// <param name="bodyHtml">Conteúdo já formatado em HTML</param>
        /// <param name="config">Configurações do servidor SMTP</param>
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml, SmtpConfiguration config);
    }
}