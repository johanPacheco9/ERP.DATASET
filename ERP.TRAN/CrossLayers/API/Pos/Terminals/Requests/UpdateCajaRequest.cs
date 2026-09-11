using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Terminals.Requests;

public sealed class UpdateCajaRequest
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la caja es obligatorio.")]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El código de la caja es obligatorio.")]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    public int StoreId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Prefix { get; set; } = "POS1";

    public long CurrentConsecutive { get; set; }

    [MaxLength(100)]
    public string? DianResolutionNumber { get; set; }

    public string? DianResolutionDate { get; set; }

    public long FromNumber { get; set; } = 1;
    public long ToNumber { get; set; } = 999999;

    public bool IsActive { get; set; } = true;
}