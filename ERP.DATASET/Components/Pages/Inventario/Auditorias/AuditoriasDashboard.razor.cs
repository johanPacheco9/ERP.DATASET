using ERP.DATA.Services.InventarioService.AuditService;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Auditorias;

public partial class AuditoriasDashboard
{
    [Inject] private AuditoriaService AuditService { get; set; } = null!;

    private bool _loading = true;
    private List<AuditSummaryDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        _items = await AuditService.ListAudits(CancellationToken.None);
        _loading = false;
    }
}
