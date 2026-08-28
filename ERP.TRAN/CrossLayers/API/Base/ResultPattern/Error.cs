namespace ERP.TRAN.CrossLayers.API.Base.ResultPattern;

public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public static Error NotFound(string code, string message) 
        => new(code, message, ErrorType.NotFound);
    
    public static Error Validation(string code, string message) 
        => new(code, message, ErrorType.Validation);
    
    public static Error Conflict(string code, string message) 
        => new(code, message, ErrorType.Conflict);
    
    public static Error Failure(string code, string message) 
        => new(code, message, ErrorType.Failure);
    
    public static Error Unauthorized(string code, string message) 
        => new(code, message, ErrorType.Unauthorized);
}