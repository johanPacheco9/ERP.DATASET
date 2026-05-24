using Microsoft.AspNetCore.Components.Forms;

namespace ERP.DATASET.Components.Pages.Inventario.NewFolder;

public sealed class EditableWarehouseRow
{
    public UpdateWarehouseForm Form { get; }
    public EditContext Context { get; }

    public bool IsValid { get; private set; }

    public EditableWarehouseRow(UpdateWarehouseForm form)
    {
        Form = form;
        Context = new EditContext(Form);
    }

    public bool IsModified => Context.IsModified();

    public void MarkValid()
    {
        IsValid = true;
    }
}
