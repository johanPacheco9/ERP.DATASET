using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
public enum TipoBodega
{
    [Display(Name = "Bodega de pérdidas")]
    Perdidas = 10,

    [Display(Name ="Bodega de bajas")]
    Bajas = 20,

    [Display(Name = "Bodega de recuperaciones")]
    Recuperacion = 30
}
