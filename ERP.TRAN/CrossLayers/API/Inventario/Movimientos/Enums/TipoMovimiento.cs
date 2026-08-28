using System.ComponentModel.DataAnnotations;
namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;

public enum TipoMovimiento
{
    [Display(Name = "Entrada")]
    Entrada = 10,

    [Display(Name = "Salida")]
    Salida = 15,

    [Display(Name = "Ajuste")]
    Ajuste = 20,

    [Display(Name = "Transferencia")]
    Transferencia = 30,

    [Display(Name = "Salida por Transferencia")]
    SalidaTransferencia = 32,

    [Display(Name = "Entrada por Transferencia")]
    EntradaTransferencia = 35,

    [Display(Name = "Movimiento de pérdidas")]
    Perdida = 40,

    [Display(Name = "Movimiento de bajas")]
    Baja = 50
}