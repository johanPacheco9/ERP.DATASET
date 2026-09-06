namespace ERP.TRAN.CrossLayers.API.Base.ResultPattern;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    // ------------------------------------------------------------------
    // NUEVO: envuelve un try/catch para evitar repetirlo en cada servicio.
    // No afecta nada existente: son métodos adicionales, no modifican
    // constructores ni firmas ya usadas.
    // ------------------------------------------------------------------

    public static Result Try(Action action, Func<Exception, Error> onException)
    {
        try
        {
            action();
            return Success();
        }
        catch (Exception ex)
        {
            return Failure(onException(ex));
        }
    }

    public static async Task<Result> TryAsync(Func<Task> action, Func<Exception, Error> onException)
    {
        try
        {
            await action();
            return Success();
        }
        catch (Exception ex)
        {
            return Failure(onException(ex));
        }
    }
}

public class Result<T> : Result
{
    private readonly T? _value;

    private Result(bool isSuccess, Error error, T? value = default)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result");

    public static Result<T> Success(T value) => new(true, Error.None, value);
    public new static Result<T> Failure(Error error) => new(false, error);

    // ------------------------------------------------------------------
    // NUEVO: mismas versiones Try/TryAsync pero para Result<T>.
    // ------------------------------------------------------------------

    public static Result<T> Try(Func<T> func, Func<Exception, Error> onException)
    {
        try
        {
            var value = func();
            return Success(value);
        }
        catch (Exception ex)
        {
            return Failure(onException(ex));
        }
    }

    public static async Task<Result<T>> TryAsync(Func<Task<T>> func, Func<Exception, Error> onException)
    {
        try
        {
            var value = await func();
            return Success(value);
        }
        catch (Exception ex)
        {
            return Failure(onException(ex));
        }
    }
}