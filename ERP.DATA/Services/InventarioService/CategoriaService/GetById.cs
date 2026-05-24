
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService
{
    public async Task<CategoriaDetailDto> GetById(GetCategoriaByIdRequest request, CancellationToken cancellationToken)
    {
        var categoria = await context.Category
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoriaDetailDto(
                c.Id,
                c.Name,
                c.Description,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
        return categoria;
    }
}
