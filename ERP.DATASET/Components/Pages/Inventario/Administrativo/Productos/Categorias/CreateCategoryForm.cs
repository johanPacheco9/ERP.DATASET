using System.ComponentModel.DataAnnotations;
namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Categorias;

public class CreateCategoryForm
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    
    [MaxLength(10, ErrorMessage = "El código no puede exceder 10 caracteres")]
    public string? Code { get; set; }
}