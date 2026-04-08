using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateProveedorEndpoint : BaseCreateEndpoint<CreateProveedorRequest, CreateProveedorEndpoint>
{
    private readonly ISupplierService _proveedorService;
    public CreateProveedorEndpoint(ILogger<CreateProveedorEndpoint> logger, ISupplierService proveedorService)
        : base(logger)
    {
        _proveedorService = proveedorService;
    }

    [Tags("Inventario - Proveedores")]
    [HttpPost(ProveedorEndpoints.List, Name = "CrearProveedor")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProveedorRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateProveedorRequest request, CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
            return BadRequest(new { errors = validationErrors });

        var proveedor = new Supplier
        {
            Name = request.Nombre,
            Nit = request.Nit,
            Address = request.Direccion,
            Phone = request.Telefono,
            IsActive = request.Activo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };

        var proveedorCreado = await _proveedorService.AddProveedorAsync(proveedor, cancellationToken);

        TraceCreated(nameof(Supplier), proveedorCreado.Id);

        return CreatedAtRoute("GetProveedorById", new { id = proveedorCreado.Id }, new
        {
            id = proveedorCreado.Id,
            nombre = proveedorCreado.Name,
            message = "Supplier creado exitosamente"
        });
    }
}