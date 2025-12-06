using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProveedorController;

public sealed class GetProveedorByIdEndpoint
    : BaseGetEndpoint<GetProveedorByIdRequest, GetProveedorByIdEndpoint, ProveedorDetailDto>
{
    private readonly IProveedorService _proveedorService;
    public GetProveedorByIdEndpoint(IProveedorService proveedorService,ILogger<GetProveedorByIdEndpoint> logger) : base(logger)
    {
        _proveedorService = proveedorService;
    }
    [Tags("Inventario - Proveedores")]
    [HttpGet(ProveedorEndpoints.Get, Name = "GetProveedorById")]
    public override async Task<ActionResult<ProveedorDetailDto>> HandleAsync(
        [FromRoute] GetProveedorByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<ProveedorDetailDto>> GetEntity(
        GetProveedorByIdRequest request, CancellationToken cancellationToken)
    {
        
        var proveedor = await _proveedorService.GetProveedorById(request.Id, cancellationToken);
        if (proveedor == null)
            return EntityNotFound(nameof(proveedor), request.Id);

        var proveedorDto = new ProveedorDetailDto
        (
            Id : proveedor.Id,
            Nombre :proveedor.Nombre,
            Nit :proveedor.Nit,
            Direccion :proveedor.Direccion,
            FechaCreacion : proveedor.CreatedAt,
            FechaActualizacion : proveedor.UpdatedAt,
            Telefono :proveedor.Telefono,
            Activo :proveedor.IsActive
        );

        TraceFound(nameof(proveedor), request.Id);
        return Ok(proveedorDto);
    }
}
