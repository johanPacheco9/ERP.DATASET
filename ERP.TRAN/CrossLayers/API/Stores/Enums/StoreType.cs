using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Stores.Enums;

public enum StoreType
{
    // Valor de seguridad: si por alguna razón llega un 0 a la base de datos
    // (por ejemplo un INSERT manual sin especificar Type, como pasó en las pruebas),
    // GetDisplayName() no revienta porque este valor sí existe en el enum.
    [Display(Name = "Sin definir")]
    Undefined = 0,

    [Display(Name = "Principal")]
    Principal = 10,

    [Display(Name = "Sucursal")]
    Sucursal = 20,

    [Display(Name = "Bodega / Centro de distribución")]
    Bodega = 30,

    [Display(Name = "Tienda en línea")]
    Online = 40,

    [Display(Name = "Franquicia")]
    Franquicia = 50,

    [Display(Name = "Punto temporal / Pop-up")]
    PopUp = 60,

    [Display(Name = "Distribuidor / Mayorista")]
    Distribuidor = 70
}