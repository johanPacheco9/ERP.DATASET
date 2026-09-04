using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
/// <summary>
/// Para permitir el seguimiento de los comentarios al generarse/actualizarse la orden de compra.
/// </summary>
public class OrdenCompraObservaciones
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public OrdenCompra OrdenCompra { get; set; } = null!;
    public string Texto { get; set; } = null!;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public OrdenCompraStatus EstadoAsociado { get; set; }
}