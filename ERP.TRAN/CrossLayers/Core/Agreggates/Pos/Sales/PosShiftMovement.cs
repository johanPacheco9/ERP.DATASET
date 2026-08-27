using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

/// <summary>
/// Registra los movimientos menores de efectivo que ocurren durante un turno de caja 
/// (diferentes a las ventas directas). 
/// Caso de uso: Permite justificar variaciones físicas en el cajón por situaciones cotidianas, 
/// como sacar dinero para comprar rollos de papel térmico (salida/gasto) o ingresar monedas 
/// adicionales para dar devueltas (entrada/adición), evitando falsos faltantes o sobrantes 
/// al momento del arqueo final.
/// </summary>
public class PosShiftMovement : EntityWithtraceability
{
    /// <summary>
    /// Identificador del turno de caja al cual pertenece este movimiento.
    /// </summary>
    public int PosShiftId { get; set; }

    /// <summary>
    /// Tipo de movimiento de efectivo (Ingreso/Adición o Salida/Gasto).
    /// </summary>
    public PosShiftMovementType Type { get; set; }

    /// <summary>
    /// Monto en dinero del movimiento.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Motivo o justificación obligatoria del movimiento (ej. "Compra de rollos de papel térmico" 
    /// o "Ingreso de sencillo adicional para cambio").
    /// </summary>
    public string Reason { get; set; } = null!;

    /// <summary>
    /// Propiedad de navegación hacia el turno de caja asociado.
    /// </summary>
    public PosShift PosShift { get; set; } = null!;
}