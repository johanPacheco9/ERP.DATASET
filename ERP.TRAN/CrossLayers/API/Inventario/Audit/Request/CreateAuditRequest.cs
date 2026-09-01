using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

/// <summary>
/// Solicitud para crear una nueva auditoría de inventario
/// </summary>
public class CreateAuditRequest : BaseCreateRequest
{
    /// <summary>
    /// Código único de la auditoría (opcional, se genera automático si no se provee)
    /// </summary>
    [MaxLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
    public string? Code { get; set; }

    /// <summary>
    /// Fecha de inicio de la auditoría
    /// </summary>
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Tipo de auditoría a realizar
    /// </summary>
    [Required(ErrorMessage = "El tipo de auditoría es requerido")]
    public AuditType Type { get; set; }

    /// <summary>
    /// ID de la bodega a auditar. Siempre obligatorio: una auditoría física
    /// se ejecuta sobre un único espacio físico, nunca sobre varias bodegas a la vez.
    /// </summary>
    [Required(ErrorMessage = "La bodega es requerida")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// ID de la categoría a auditar. Requerido cuando Type es Cyclical (método ABC).
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// ID del producto específico a auditar. Requerido cuando Type es Selective o PostMovement
    /// (conteo dirigido: reclamo de cliente, descuadre puntual, verificación tras un movimiento).
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// ID del responsable de ejecutar la auditoría
    /// </summary>
    [Required(ErrorMessage = "El responsable es requerido")]
    public int ResponsibleId { get; set; }

    /// <summary>
    /// ID del supervisor que aprobará la auditoría (opcional)
    /// </summary>
    public int? SupervisorId { get; set; }

    /// <summary>
    /// Observations iniciales de la auditoría
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observations { get; set; }

    /// <summary>
    /// Incluir unidades apartadas en la auditoría (además de las disponibles)
    /// Por defecto: true
    /// </summary>
    public bool IncludeReservedUnits { get; set; } = true;

    public override bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        // Validar fecha de inicio
        if (StartDate == default)
        {
            errorList.Add("La fecha de inicio no puede estar vacía.");
        }

        if (StartDate > DateTime.UtcNow.AddDays(1))
        {
            errorList.Add("La fecha de inicio no puede ser más de 1 día en el futuro.");
        }

        // Validar tipo de auditoría
        if (!Enum.IsDefined(typeof(AuditType), Type))
        {
            errorList.Add("El tipo de auditoría no es válido.");
        }

        // Validar responsable
        if (ResponsibleId == 0)
        {
            errorList.Add("El responsable es requerido.");
        }

        // Validar bodega: siempre obligatoria
        if (WarehouseId <= 0)
        {
            errorList.Add("Debe especificar una bodega para auditar.");
        }

        // Reglas de alcance según el tipo de auditoría.
        // Nota: General, Surprise, Monthly y Annual no imponen alcance específico
        // (pueden auditar toda la bodega, o combinarse con CategoryId/ProductId a discreción).
        switch (Type)
        {
            case AuditType.Cyclical when !CategoryId.HasValue:
                errorList.Add("La auditoría cíclica requiere especificar una categoría (método ABC).");
                break;

            case AuditType.Selective when !ProductId.HasValue:
                errorList.Add("La auditoría selectiva requiere especificar un producto.");
                break;

            case AuditType.PostMovement when !ProductId.HasValue:
                errorList.Add("La auditoría post-movimiento requiere especificar el producto afectado.");
                break;
        }

        // Validar código si se proporciona
        if (!string.IsNullOrWhiteSpace(Code))
        {
            if (Code.Length > 50)
            {
                errorList.Add("El código no puede exceder 50 caracteres.");
            }

            // Validar formato: AUD-2026-001
            if (!System.Text.RegularExpressions.Regex.IsMatch(Code, @"^AUD-\d{4}-\d{3,}$"))
            {
                errorList.Add("El código debe tener el formato AUD-YYYY-NNN (ej: AUD-2026-001).");
            }
        }

        errors = errorList.Any() ? string.Join("; ", errorList) : null;
        return string.IsNullOrEmpty(errors);
    }
}