using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

/// <summary>
/// Representa a un usuario del sistema (empleado, cajero, administrador), 
/// encargado de operar las terminales y realizar las transacciones.
/// </summary>
public class Usuario : EntityWithtraceability
{
    /// <summary>
    /// Nombre de usuario calculado automáticamente (ej. PrimerNombre + PrimerApellido en minúsculas y sin espacios).
    /// Al no tener 'set', EF Core lo ignora por completo en la base de datos.
    /// </summary>
    public string UserName => $"{PrimerNombre?.Trim()}{PrimerAPellido?.Trim()}".ToLowerInvariant();

    /// <summary>
    /// Correo electrónico institucional o de contacto del usuario.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Nombres y apellidos del usuario.
    /// </summary>
    public string PrimerNombre { get; set; } = null!;
    
    public string? SegundoNombre { get; set; }

    public string PrimerAPellido { get; set; } = null!;
    
    public string? SegundoAPellido { get; set; }
    
    /// <summary>
    /// Hash de la contraseña del usuario para la autenticación.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Indica si el usuario se encuentra activo para operar en el sistema.
    /// </summary>
    public new bool IsActive { get; set; } = true;
    
    public UserRole Role { get; set; }

    /// <summary>
    /// Colección de turnos (sesiones de caja) operados por este usuario.
    /// </summary>
    public ICollection<PosShift> PosShifts { get; set; } = new List<PosShift>();
}