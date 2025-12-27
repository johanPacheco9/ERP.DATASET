namespace ERP.DATASET.Components.Generics;

public abstract class BaseCreateForm
{
    public List<string> Errors { get; } = new();

    public bool IsValid => Errors.Count == 0;

    protected void AddError(string message)
    {
        Errors.Add(message);
    }

    public void ClearErrors()
    {
        Errors.Clear();
    }
    public abstract void Validate();
}
