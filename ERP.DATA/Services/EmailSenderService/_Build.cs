using Resend;
using ERP.TRAN.CrossLayers.API.EmailSender.Requests;
using ERP.TRAN.CrossLayers.API.EmailSender.Responses;

namespace ERP.DATA.Services.EmailSenderService;

public partial class EmailSenderService(IResend resend)
{
    private const string DefaultFromEmail = "onboarding@resend.dev";

    public async Task<SendEmailResponse> Send(
        SendEmailRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var displayName = string.IsNullOrWhiteSpace(request.FromName) ? "ERP Sistema" : request.FromName;
            var fromAddress = $"{displayName} <{DefaultFromEmail}>";

            var message = new EmailMessage
            {
                From = fromAddress,
                To = { request.To },
                Subject = request.Subject,
                HtmlBody = request.HtmlBody
            };

            var apiResponse = await resend.EmailSendAsync(message, cancellationToken);

            return new SendEmailResponse
            {
                IsSuccess = true,
                MessageId = "Enviado",
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Error]: Falló el envío a {request.To}. Motivo: {ex.Message}");
            
            return new SendEmailResponse
            {
                IsSuccess = false,
                MessageId = null,
                ErrorMessage = ex.Message
            };
        }
    }
}