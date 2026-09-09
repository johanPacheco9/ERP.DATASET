using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Users.Enums;

/// <summary>
/// Define los roles o perfiles de seguridad disponibles para los usuarios del sistema.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrador con acceso total a la configuración y módulos del ERP.
    /// </summary>
    [Display(Name = "Admin")]
    Admin = 15,

    /// <summary>
    /// Cajero autorizado para abrir/cerrar turnos y realizar ventas en el POS.
    /// </summary>
    [Display(Name = "Cajero")]
    Cashier = 30,

    /// <summary>
    /// Supervisor o jefe de tienda con permisos especiales para anulaciones y arqueos.
    /// </summary>
    [Display(Name = "Supervisor")]
    Supervisor = 35
}