using ERP.DATA.Services.InventarioService.AuditService;
using ERP.DATA.Services.InventarioService.UnidadProductoService;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATASET.Components.Pages.Inventario.Auditorias;

public partial class AuditoriaDetalle
{
    [Parameter] public int Id { get; set; }

    [Inject] private AuditoriaService AuditService { get; set; } = null!;
    [Inject] private UnidadProductoManager UnidadProductoManager { get; set; } = null!;
    [Inject] private ProductoBaseService ProductoBaseService { get; set; } = null!;

    private bool _loading = true;
    private AuditDetailDto? _audit;
    private List<UnitProductAuditSummaryDto> _unidades = new();

    private string? _scanInput;
    private int _physicalWarehouseId;
    private string? _feedbackMessage;
    private string _feedbackClass = "alert-info";
    private int _porcentajeConteo;

    private bool _showEditModal;
    private bool _showSurplusModal;
    private bool _showCloseModal;
    private bool _savingSurplus;
    private bool _closingAudit;

    private UnitProductAuditSummaryDto? _selectedUnit;
    private UpdateUnitAuditedProductForm _editForm = new();
    private SurplusUnitForm _surplusForm = new();
    private CloseAuditForm _closeForm = new();
    private List<ProductoSummaryDto> _productos = new();
    private List<ProductoVarianteDetailDto> _surplusVariants = new();

    private bool AuditIsClosed =>
        _audit?.StatusDisplay.Contains("Completada", StringComparison.OrdinalIgnoreCase) == true ||
        _audit?.StatusDisplay.Contains("Rechazada", StringComparison.OrdinalIgnoreCase) == true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadData();
            if (_audit != null) _physicalWarehouseId = _audit.WarehouseId ?? 0;
            await LoadProducts();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error critico en inicializacion: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadData()
    {
        _audit = await AuditService.GetAuditById(Id, CancellationToken.None);
        if (_audit == null) return;

        _unidades = await AuditService.ListUnitAuditedProducts(Id, CancellationToken.None);
        _unidades = _unidades
            .OrderBy(u => GetStatusOrder(u.StatusDisplay))
            .ThenBy(u => u.Serial)
            .ToList();

        _porcentajeConteo = _audit.TotalExpectedUnits > 0
            ? (int)Math.Round((double)(_audit.TotalCountedUnits * 100) / _audit.TotalExpectedUnits)
            : 0;
    }

    private async Task LoadProducts()
    {
        var result = await ProductoBaseService.ListAsync(
            request: new ListProductRequest(pageNumber: 1, pageSize: 1),
            searchTerm: null,
            categoryName: null,
            stockFilter: null,
            cancellationToken: CancellationToken.None
        );
        
        _productos = result.ToList();
    }

    private async Task HandleScanKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") await ProcessScan();
    }

    private async Task ProcessScan()
    {
        if (string.IsNullOrWhiteSpace(_scanInput) || AuditIsClosed) return;

        var request = new RegisterFoundUnitsRequest
        {
            AuditId = Id,
            PhysicalWarehouseId = _physicalWarehouseId,
            ProductsIds = new List<string> { _scanInput.Trim() }
        };

        try
        {
            var result = await AuditService.RegisterFoundUnits(request, CancellationToken.None);
            if (result.Successful > 0)
            {
                _feedbackMessage = $"{result.Items.First().Message} (Serial: {_scanInput})";
                _feedbackClass = "alert-success";
                await LoadData();
            }
            else
            {
                _feedbackMessage = $"Aviso: {result.Items.First().Message}";
                _feedbackClass = "alert-warning";
            }
        }
        catch (Exception ex)
        {
            _feedbackMessage = $"Error operativo: {ex.Message}";
            _feedbackClass = "alert-danger";
        }

        _scanInput = string.Empty;
    }

    private void OpenEditUnitModal(UnitProductAuditSummaryDto unit)
    {
        if (AuditIsClosed) return;

        _selectedUnit = unit;
        _editForm = new UpdateUnitAuditedProductForm
        {
            Id = unit.Id,
            AuditId = Id,
            Status = ParseStatusDisplay(unit.StatusDisplay),
            UbicacionFisica = unit.UbicacionFisica,
            EstadoFisico = unit.EstadoFisico,
            Observaciones = unit.Observaciones,
            MotivoDiferencia = unit.MotivoDiferencia,
            RequiereAccionCorrectiva = unit.RequiereAccionCorrectiva
        };
        _showEditModal = true;
    }

    private async Task SaveUnitProductAudit()
    {
        if (_selectedUnit == null) return;

        try
        {
            var transationalRequest = new UpdateUnitProductAuditRequest("auth0|operario_bodega_actual")
            {
                Id = _editForm.Id,
                AuditId = _editForm.AuditId,
                Status = _editForm.Status,
                UbicacionFisica = _editForm.UbicacionFisica,
                EstadoFisico = _editForm.EstadoFisico,
                Observaciones = _editForm.Observaciones,
                MotivoDiferencia = _editForm.MotivoDiferencia,
                RequiereAccionCorrectiva = _editForm.RequiereAccionCorrectiva
            };

            var updatedDto = await UnidadProductoManager.Update(transationalRequest, CancellationToken.None);

            if (updatedDto != null)
            {
                _showEditModal = false;
                _feedbackMessage = $"Unidad {_selectedUnit.Serial} actualizada por el operador.";
                _feedbackClass = "alert-success";
                await LoadData();
            }
            else
            {
                _feedbackMessage = "El servidor rechazo el procesamiento de la unidad.";
                _feedbackClass = "alert-danger";
            }
        }
        catch (Exception ex)
        {
            _feedbackMessage = $"Error al actualizar: {ex.Message}";
            _feedbackClass = "alert-danger";
        }
    }

    private void TriggerAddSurplusModal()
    {
        if (AuditIsClosed) return;

        _surplusForm = new SurplusUnitForm
        {
            PhysicalWarehouseId = _audit?.WarehouseId ?? _physicalWarehouseId
        };
        _surplusVariants = new List<ProductoVarianteDetailDto>();
        _showSurplusModal = true;
    }

    private void OnSurplusProductChanged(ChangeEventArgs e)
    {
        _surplusForm.ProductId = int.TryParse(e.Value?.ToString(), out var productId)
            ? productId
            : 0;
        _surplusForm.ProductoVariantId = null;
        _surplusVariants = _productos
            .FirstOrDefault(p => p.Id == _surplusForm.ProductId)?
            .ProductoVariantes
            .Where(v => v.Activo)
            .ToList() ?? new List<ProductoVarianteDetailDto>();
    }

    private async Task SaveSurplusUnit()
    {
        if (_audit == null) return;

        if (string.IsNullOrWhiteSpace(_surplusForm.Code) ||
            _surplusForm.ProductId <= 0 ||
            !_surplusForm.ProductoVariantId.HasValue ||
            _surplusForm.PhysicalWarehouseId <= 0)
        {
            _feedbackMessage = "Complete serial, producto, variante y bodega para registrar el sobrante.";
            _feedbackClass = "alert-warning";
            return;
        }

        _savingSurplus = true;
        try
        {
            var result = await AuditService.RegisterSurplusUnit(new RegisterSurplusUnitRequest
            {
                AuditId = Id,
                Code = _surplusForm.Code.Trim(),
                ProductId = _surplusForm.ProductId,
                ProductoVariantId = _surplusForm.ProductoVariantId,
                PhysicalWarehouseId = _surplusForm.PhysicalWarehouseId,
                Observations = _surplusForm.Observations,
                _AuditorAuth0Id = "auth0|operario_bodega_actual"
            }, CancellationToken.None);

            _showSurplusModal = false;
            _feedbackMessage = $"Sobrante {result.Serial} registrado para conciliacion.";
            _feedbackClass = "alert-success";
            await LoadData();
        }
        catch (Exception ex)
        {
            _feedbackMessage = $"Error al registrar sobrante: {ex.Message}";
            _feedbackClass = "alert-danger";
        }
        finally
        {
            _savingSurplus = false;
        }
    }

    private void OpenCloseAuditModal()
    {
        if (_audit == null || AuditIsClosed) return;

        _closeForm = new CloseAuditForm { Conclusions = _audit.Conclusions };
        _showCloseModal = true;
    }

    private async Task CloseAudit()
    {
        _closingAudit = true;
        try
        {
            await AuditService.CloseAudit(new CloseAuditRequest
            {
                AuditId = Id,
                Conclusions = _closeForm.Conclusions,
                _CloserAuth0Id = "auth0|supervisor_actual"
            }, CancellationToken.None);

            _showCloseModal = false;
            _feedbackMessage = "Auditoria cerrada correctamente.";
            _feedbackClass = "alert-success";
            await LoadData();
        }
        catch (Exception ex)
        {
            _feedbackMessage = $"Error al cerrar auditoria: {ex.Message}";
            _feedbackClass = "alert-danger";
        }
        finally
        {
            _closingAudit = false;
        }
    }

    private static string _GetStatusClass(string status)
    {
        if (string.IsNullOrEmpty(status)) return "bg-light text-dark border-secondary-subtle";
        var lowerStatus = status.ToLower();
        if (lowerStatus.Contains("no encontrado") || lowerStatus.Contains("notfound") || lowerStatus.Contains("pendiente"))
            return "bg-danger-subtle text-danger border-danger-subtle";
        if (lowerStatus.Contains("sobrante") || lowerStatus.Contains("excess") || lowerStatus.Contains("exceso"))
            return "bg-warning-subtle text-warning border-warning-subtle";
        if (lowerStatus.Contains("mismatch") || lowerStatus.Contains("diferencia"))
            return "bg-info-subtle text-info border-info-subtle";
        if (lowerStatus.Contains("encontrado") || lowerStatus.Contains("found") || lowerStatus.Contains("correcto"))
            return "bg-success-subtle text-success border-success-subtle";
        return "bg-light text-dark border-secondary-subtle";
    }

    private static UnitProductAuditStatus ParseStatusDisplay(string statusDisplay)
    {
        foreach (UnitProductAuditStatus status in Enum.GetValues(typeof(UnitProductAuditStatus)))
        {
            if (string.Equals(status.ToString(), statusDisplay, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status.GetDisplayName(), statusDisplay, StringComparison.OrdinalIgnoreCase))
            {
                return status;
            }
        }

        return UnitProductAuditStatus.NotFound;
    }

    private static int GetStatusOrder(string statusDisplay)
    {
        var status = ParseStatusDisplay(statusDisplay);
        return status switch
        {
            UnitProductAuditStatus.NotFound => 0,
            UnitProductAuditStatus.StatusMismatch => 1,
            UnitProductAuditStatus.ExcessProduct => 2,
            UnitProductAuditStatus.Found => 3,
            _ => 4
        };
    }

    private sealed class SurplusUnitForm
    {
        public string? Code { get; set; }
        public int ProductId { get; set; }
        public int? ProductoVariantId { get; set; }
        public int PhysicalWarehouseId { get; set; }
        public string? Observations { get; set; }
    }

    private sealed class CloseAuditForm
    {
        public string? Conclusions { get; set; }
    }
}
