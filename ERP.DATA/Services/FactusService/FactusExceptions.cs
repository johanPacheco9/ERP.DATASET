using System.Net;

namespace ERP.DATA.Services.FactusService;

/// <summary>Base para errores de la integración con Factus.</summary>
public class FactusException : Exception
{
    public FactusException(string message) : base(message) { }
    public FactusException(string message, Exception inner) : base(message, inner) { }
}

public class FactusAuthException : FactusException
{
    public FactusAuthException(string message) : base(message) { }
    public FactusAuthException(string message, Exception inner) : base(message, inner) { }
}

public class FactusValidationException : FactusException
{
    public IReadOnlyList<string> Errors { get; }
    public FactusValidationException(string message, IEnumerable<string>? errors = null) : base(message)
    {
        Errors = (errors ?? Array.Empty<string>()).ToList();
    }
}

public class FactusConflictException : FactusException
{
    public string? ReferenceCode { get; }
    public bool IsValidated { get; }
    public FactusConflictException(string message, string? referenceCode = null, bool isValidated = false)
        : base(message)
    {
        ReferenceCode = referenceCode;
        IsValidated = isValidated;
    }
}

public class FactusRateLimitException : FactusException
{
    public TimeSpan? RetryAfter { get; }
    public FactusRateLimitException(string message, TimeSpan? retryAfter = null) : base(message)
    {
        RetryAfter = retryAfter;
    }
}

public class FactusServerException : FactusException
{
    public HttpStatusCode? StatusCode { get; }
    public FactusServerException(string message, HttpStatusCode? statusCode = null) : base(message)
    {
        StatusCode = statusCode;
    }
}
