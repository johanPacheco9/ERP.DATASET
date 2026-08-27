using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

/// <summary>
/// Representa el turno o sesión de caja. Registra la información financiera, de control 
/// y los movimientos de efectivo ocurridos durante el periodo de trabajo de un cajero en una terminal POS.
/// </summary>
public class PosShift : EntityWithtraceability
{
    /// <summary>
    /// Identificador de la caja física (terminal POS) donde se abrió el turno.
    /// </summary>
    public int PosTerminalId { get; set; }

    /// <summary>
    /// Identificador único del usuario (cajero) que abrió y opera el turno.
    /// </summary>
    public int CajeroId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia la entidad de usuarios del sistema.
    /// </summary>
    public Usuario Usuarios { get; set; } = null!;

    /// <summary>
    /// Fecha y hora exacta en la que se abrió el turno (UTC).
    /// </summary>
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora exacta en la que se cerró el turno (Nulo si la caja sigue abierta).
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Dinero en efectivo base (fondo de cambio) con el que arranca la caja al abrir el turno.
    /// </summary>
    public decimal InitialCash { get; set; }

    /// <summary>
    /// Total acumulao de ventas cobradas en efectivo durante el turno.
    /// </summary>
    public decimal CashSales { get; set; }

    /// <summary>
    /// Total acumulado de ventas cobradas con tarjeta (débito/crédito) durante el turno.
    /// </summary>
    public decimal CardSales { get; set; }

    /// <summary>
    /// Total acumulado de ventas cobradas por transferencia electrónica durante el turno.
    /// </summary>
    public decimal TransferSales { get; set; }

    /// <summary>
    /// Total acumulado de ventas realizadas a crédito durante el turno.
    /// </summary>
    public decimal CreditSales { get; set; }

    /// <summary>
    /// Salidas menores de dinero en efectivo de la caja durante el turno (ej. gastos operativos, pagos rápidos).
    /// </summary>
    public decimal CashWithdrawals { get; set; }

    /// <summary>
    /// Entradas adicionales de dinero en efectivo ingresadas a la caja durante el turno (ej. inyección de cambio extra).
    /// </summary>
    public decimal CashAdditions { get; set; }

    /// <summary>
    /// Efectivo total esperado en el cajón al momento del cierre (Calculado: Base Inicial + Ventas en Efectivo + Entradas - Retiros).
    /// </summary>
    public decimal TotalExpectedCash { get; set; }

    /// <summary>
    /// Efectivo físico real contado por el cajero al realizar el arqueo de cierre de caja.
    /// </summary>
    public decimal? ActualCash { get; set; }

    /// <summary>
    /// Diferencia resultante en el arqueo (Calculada: Efectivo Real Contado menos Efectivo Esperado). 
    /// Valores positivos indican sobrante, negativos indican faltante y cero indica caja cuadrada.
    /// </summary>
    public decimal? Difference { get; set; }

    /// <summary>
    /// Estado actual del turno (ej. Abierto, Cerrado, Auditado).
    /// </summary>
    public PosShiftStatus Status { get; set; } = PosShiftStatus.Open;

    /// <summary>
    /// Observaciones, novedades o justificaciones registradas durante el turno o al momento del cierre.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Propiedad de navegación a la terminal POS (caja física) asociada a este turno.
    /// </summary>
    public PosTerminal PosTerminal { get; set; } = null!;

    /// <summary>
    /// Colección de ventas realizadas y amarradas a este turno específico de caja.
    /// </summary>
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    
    /// <summary>
    /// Colección de movimientos menores de efectivo registrados durante este turno.
    /// </summary>
    public ICollection<PosShiftMovement> Movements { get; set; } = new List<PosShiftMovement>();
}