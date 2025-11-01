using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ERP.API.Controllers.Api.v1.Inventario.ProveedorController;

public sealed class GetProveedorByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetProveedorByIdRequest, GetProveedorByIdEndpoint, ProveedorDetailDto>(serviceProvider)
{
    [Tags("Inventario - Proveedores")]
    [HttpGet(ProveedorEndpoints.Get, Name = ("GetProveedorById"))]
    public override async Task<ActionResult<ProveedorDetailDto>> HandleAsync(
        [FromRoute] GetProveedorByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<ProveedorDetailDto>> GetEntity(GetProveedorByIdRequest request, CancellationToken cancellationToken)
    {

        var proveedor = await Repository.Proveedores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (proveedor is null)
            return EntityNotFound(nameof(Proveedor));

        var proveedorDto = new ProveedorDetailDto
        (
            Id: proveedor.Id,
            Nombre: proveedor.Nombre,
            Nit : proveedor.Nit,
            Direccion: proveedor.Direccion,
            FechaCreacion: proveedor.CreatedAt,
            FechaActualizacion: proveedor.UpdatedAt,
            Telefono: proveedor.Telefono,
            Activo: proveedor.IsActive
        );

        TraceFound(nameof(Proveedor), request.Id);

        return proveedorDto;
    }
}







