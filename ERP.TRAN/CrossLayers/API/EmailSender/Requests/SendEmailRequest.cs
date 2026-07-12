namespace ERP.TRAN.CrossLayers.API.EmailSender.Requests;

public class SendEmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? FromName { get; set; }
}