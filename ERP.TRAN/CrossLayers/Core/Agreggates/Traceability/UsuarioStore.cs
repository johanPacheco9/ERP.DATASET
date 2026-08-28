using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

/// <summary>
/// Tabla intermedia (Join Table) que representa la relación de muchos a muchos 
/// entre los usuarios del sistema y las tiendas/sucursales.
/// </summary>
/// <remarks>
/// Su objetivo es soportar la rotación de empleados entre diferentes sucursales, 
/// controlando qué usuarios están autorizados para operar en qué tiendas específicas.
/// </remarks>
public class UsuarioStore
{
    public int Id { get; set; }
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public int UsuarioId { get; set; }
    
    /// <summary>
    /// Entidad de navegación hacia el usuario.
    /// </summary>
    public Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Identificador único de la tienda o sucursal.
    /// </summary>
    public int StoreId { get; set; }
    
    /// <summary>
    /// Entidad de navegación hacia la tienda.
    /// </summary>
    public Store Store { get; set; } = null!;

    /// <summary>
    /// Indica si esta es la tienda base o principal asignada por defecto al usuario 
    /// cuando inicia sesión y tiene acceso a múltiples sucursales.
    /// </summary>
    public bool IsDefault { get; set; }
}