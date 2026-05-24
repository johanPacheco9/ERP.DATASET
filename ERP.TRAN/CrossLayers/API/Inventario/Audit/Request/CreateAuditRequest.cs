using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;
using System.ComponentModel.DataAnnotations;

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
    /// ID de la bodega a auditar (null = todas las bodegas)
    /// </summary>
    public int? WarehouseId { get; set; }

    /// <summary>
    /// ID de la categoría a auditar (null = todas las categorías)
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// ID del producto específico a auditar (null = todos los productos)
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// ID del responsable de ejecutar la auditoría
    /// </summary>
    [Required(ErrorMessage = "El responsable es requerido")]
    // [MaxLength(100)]  ← ELIMINADO ❌
    public int ResponsibleId { get; set; }

    /// <summary>
    /// ID del supervisor que aprobará la auditoría (opcional)
    /// </summary>
    // [MaxLength(100)]  ← ELIMINADO ❌
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

        // Validar alcance: no puede tener ProductId sin WarehouseId
        if (ProductId.HasValue && !WarehouseId.HasValue)
        {
            errorList.Add("Si especifica un producto, debe especificar también una bodega.");
        }

        // Al menos debe especificar una bodega para auditorías
        if (!WarehouseId.HasValue)
        {
            errorList.Add("Debe especificar al menos una bodega para auditar.");
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