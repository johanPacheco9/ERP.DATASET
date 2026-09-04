using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Marca.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.MarcasService;

public partial class MarcasManager
{
    public async Task<Result<List<MarcasDetailDto>>> List(CancellationToken cancellationToken = default)
    {
        var marcas = await _context.Marca
            .AsNoTracking()
            .Select(m => new MarcasDetailDto(
                m.Id,
                m.Nombre,
                m.Descripcion,
                m.LogoUrl,
                m.Activa,
                m.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<List<MarcasDetailDto>>.Success(marcas);
    }
}