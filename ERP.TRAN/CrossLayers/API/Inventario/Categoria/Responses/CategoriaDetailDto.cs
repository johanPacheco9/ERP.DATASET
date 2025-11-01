namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;

public class CategoriaDetailDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
};

