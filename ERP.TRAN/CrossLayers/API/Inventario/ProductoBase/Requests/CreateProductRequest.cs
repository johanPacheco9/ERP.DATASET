using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;

public sealed class CreateProductoRequest : BaseCreateRequest, IValidatableRequest
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Costo_Unitario { get; set; }
    public decimal Precio_Venta { get; set; }
    public decimal PorcentajeIVA { get; set; } = 0.19m;
    public decimal PorcentajeICA { get; set; }
    public decimal ImpuestoEspecifico { get; set; }
    public decimal ArancelImportacion { get; set; }
    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }
    public string? CodigoTributario { get; set; }
    public int CategoriaId { get; set; }
    public int? ProveedorId { get; set; }
    
    public int? BodegaId { get; set; } //Si no se manda, se usará la bodega principal. 
    public string Unidad_Medida { get; set; } = "UND";
    public decimal Peso { get; set; }
    public decimal Volumen { get; set; }
    public string? Dimensiones { get; set; }
    public string? Imagen_Url { get; set; }
    public string? Notas { get; set; }
    public string? Tags { get; set; }
    public ProductoBaseStatus Estado { get; set; } = ProductoBaseStatus.Active;
    public bool HasVariantes { get; set; }
    public bool EsPerecedero { get; set; }
    public DateTime? FechaCaducidad { get; set; }
    public List<CreateProductoVarianteRequest>? Variantes { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();

        if (string.IsNullOrWhiteSpace(Codigo))
            list.Add("El código es obligatorio.");
        else if (Codigo.Length < 3 || Codigo.Length > 12)
            list.Add("El código debe tener entre 3 y 12 caracteres.");

        if (string.IsNullOrWhiteSpace(Nombre))
            list.Add("El nombre es obligatorio.");

        if (CategoriaId <= 0)
            list.Add("Debe asignar una categoría.");

        if (Precio_Venta < 0)
            list.Add("El precio no puede ser negativo.");

        if (Costo_Unitario < 0)
            list.Add("El costo no puede ser negativo.");

        if (PorcentajeIVA is < 0 or > 1)
            list.Add("El IVA debe estar entre 0 y 1.");

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}