namespace ERP.TRAN.CrossLayers.API.EmailSender.Responses;

public class SendEmailResponse
{
    public bool IsSuccess { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
}