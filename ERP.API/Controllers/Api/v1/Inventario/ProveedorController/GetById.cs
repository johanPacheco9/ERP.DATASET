using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProveedorController;

public sealed class GetProveedorByIdEndpoint
    : BaseGetEndpoint<GetProveedorByIdRequest, GetProveedorByIdEndpoint, ProveedorDetailDto>
{
    private readonly ISupplierService _proveedorService;
    public GetProveedorByIdEndpoint(ISupplierService proveedorService,ILogger<GetProveedorByIdEndpoint> logger) : base(logger)
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
            Nombre :proveedor.Name,
            Nit :proveedor.Nit,
            Direccion :proveedor.Address,
            FechaCreacion : proveedor.CreatedAt,
            FechaActualizacion : proveedor.UpdatedAt,
            Telefono :proveedor.Phone,
            Activo :proveedor.IsActive
        );

        TraceFound(nameof(proveedor), request.Id);
        return Ok(proveedorDto);
    }
}
