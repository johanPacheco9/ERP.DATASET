using ERP.DATA.Services.InventarioService.AuditService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Auditorias;

public partial class CrearAuditoria
{
    [Inject] private AuditoriaService AuditService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private readonly CrearAuditoriaForm _form = new();
    private List<WarehouseSummaryDto> _bodegas = new();
    private bool _saving;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        var result = await WarehouseService.List(
            new ListWarehousesRequest { PageNumber = 1, PageSize = 100 },
            CancellationToken.None);
        _bodegas = result.ToList();
        _form.StartDate = DateTime.Today;
        _form.Type = AuditType.General;
        _form.ResponsibleId = 1;
    }

    private async Task Crear()
    {
        _error = null;
        if (_form.WarehouseId <= 0)
        {
            _error = "Seleccione una bodega.";
            return;
        }

        _saving = true;
        try
        {
            var request = new CreateAuditRequest
            {
                StartDate = _form.StartDate,
                Type = _form.Type,
                WarehouseId = _form.WarehouseId,
                ResponsibleId = _form.ResponsibleId,
                Observations = _form.Observations,
                IncludeReservedUnits = _form.IncludeReservedUnits,
                _CreatorAuth0Id = 1
            };

            var audit = await AuditService.CreateAudit(request, CancellationToken.None);
            Navigation.NavigateTo($"/auditorias/{audit.Id}");
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _saving = false;
        }
    }

    private sealed class CrearAuditoriaForm
    {
        public int WarehouseId { get; set; }
        public AuditType Type { get; set; }
        public DateTime StartDate { get; set; }
        public int ResponsibleId { get; set; } = 1;
        public string? Observations { get; set; }
        public bool IncludeReservedUnits { get; set; } = true;
    }
}
