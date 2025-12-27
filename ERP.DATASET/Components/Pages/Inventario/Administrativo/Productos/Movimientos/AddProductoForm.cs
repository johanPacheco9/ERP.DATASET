using ERP.DATASET.Components.Generics;

public sealed class AddProductoForm : BaseCreateForm
{
    public Guid CategoriaId { get; set; }

    public int CantidadAIngresar { get; set; }

    public string NombreProducto { get; set; } = string.Empty;

    public string? DescripcionProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public decimal CostoUnitario { get; set; }
    public decimal PrecioVenta { get; set; }
    public override void Validate()
    {
        ClearErrors();

        if (CategoriaId == Guid.Empty)
            AddError("La categoría es requerida.");

        if (CantidadAIngresar <= 0)
            AddError("La cantidad debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(NombreProducto))
            AddError("El nombre del producto es obligatorio.");

        if (NombreProducto.Length > 120)
            AddError("El nombre no puede superar 120 caracteres.");

        if (string.IsNullOrWhiteSpace(Codigo))
            AddError("El código es obligatorio.");

        if (Codigo.Length < 3 || Codigo.Length > 12)
            AddError("El código debe tener entre 3 y 12 caracteres.");

        if (PrecioVenta < CostoUnitario)
            AddError("El precio de venta no puede ser menor al costo.");
    }
}
