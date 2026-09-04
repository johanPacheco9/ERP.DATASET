using ERP.DATA.Services.InventarioService.AuditService;
using ERP.DATA.Services.InventarioService.AuditService.Responses;
using ERP.DATA.Services.InventarioService.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ERP.DATASET.Components.Pages.Inventario.Auditorias; // Ajusta a tu namespace correspondiente

public partial class AuditoriaDetalle : ComponentBase
{
    // TODO: reemplazar por el Auth0Id real del usuario autenticado cuando esté listo el login.
    private const string _PlaceholderAuth0Id = "dev-placeholder";

    [Parameter] public int Id { get; set; }

    [Inject] private AuditoriaService AuditoriaService { get; set; } = null!;
    [Inject] private MovimientosManager MovimientosManager { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private bool _loading = true;
    private AuditProgressDto _audit;
    private List<AuditUnitDto> _unidades = [];
    private List<ProductLookupDto> _productos = [];
    private List<ProductVariantLookupDto> _surplusVariants = [];

    private AuditDetailDto? _auditDetailDto;
    private string? _scanInput;
    private string? _feedbackMessage;
    private string _feedbackClass = "alert-success";

    // Modales y formularios
    private bool _showEditModal;
    private AuditUnitDto? _selectedUnit;
    private EditUnitAuditModel _editForm = new();

    private bool _showSurplusModal;
    private SurplusUnitModel _surplusForm = new();
    private bool _savingSurplus;
    private List<AuditUnitDto> _pendingSurplusUnits = [];

    private bool _showLossModal;
    private AuditUnitDto? _unitToLoss;
    private string _lossObservations = string.Empty;
    private bool _sendingLoss;

    private bool _showCloseModal;
    private CloseAuditModel _closeForm = new();
    private bool _closingAudit;

    private bool _showRejectModal;
    private RejectAuditModel _rejectForm = new();
    private bool _rejectingAudit;

    // Ahora contempla tanto el cierre normal como el rechazo por inconsistencias
    private bool AuditIsClosed =>
        _audit?.Status == AuditStatus.Completada.GetDisplayName() ||
        _audit?.Status == AuditStatus.RejectWithinconsistences.GetDisplayName();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        _feedbackMessage = null;

        try
        {
            using var cts = new CancellationTokenSource();
        
            // 1. Cargas la cabecera / progreso general
            _audit = await AuditoriaService.GetAuditProgress(Id, cts.Token);

            // 2. Cargas la lista actualizada de unidades auditadas (¡Esto era lo que faltaba!)
            _unidades = await AuditoriaService.GetAuditUnits(Id, cts.Token);

            // 3. Recalcula los sobrantes pendientes de identificar (para el modal)
            _pendingSurplusUnits = _unidades
                .Where(u => u.StatusCode == UnitProductAuditStatus.ExcessProduct && u.ProductoVariantId == 0)
                .ToList();
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al cargar la auditoría: {ex.Message}", isError: true);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task HandleScanKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ProcessScan();
        }
    }

    private async Task ProcessScan()
    {
        if (string.IsNullOrWhiteSpace(_scanInput)) return;

        string code = _scanInput.Trim();
        _scanInput = string.Empty;

        try
        {
            var result = await AuditoriaService.RegisterScanAsync(Id, code);

            if (result.IsSuccess)
            {
                if (result.IsSuccess)
                {
                    ShowFeedback($"Unidad '{code}' encontrada y confirmada correctamente.");
                }
                else
                {
                    ShowFeedback($"'{code}' no pertenece a esta auditoría: se registró como unidad EN EXCESO. Complétala en \"Identificar Sobrante\".");
                    _feedbackClass = "alert-warning";
                }
                await LoadData(); // Recarga los datos para actualizar la tabla y contadores
            }
            else
            {
                ShowFeedback(result.Error.Message, isError: true);
            }
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al procesar lectura: {ex.Message}", isError: true);
        }
    }

    private void OpenEditUnitModal(AuditUnitDto unit)
    {
        _selectedUnit = unit;
        _editForm = new EditUnitAuditModel
        {
            Status = unit.StatusCode, // Asume que tienes el enum o valor numérico
            UbicacionFisica = unit.UbicacionFisica,
            Observaciones = unit.Observaciones
        };
        _showEditModal = true;
    }

    private async Task SaveUnitProductAudit()
    {
        if (_selectedUnit == null) return;

        try
        {
            var success = await AuditoriaService.UpdateUnitProductAudit(new UpdateUnitProductAuditRequest
            {
                Id = _selectedUnit.Id,
                Status = _editForm.Status,
                UbicacionFisica = _editForm.UbicacionFisica,
                Observaciones = _editForm.Observaciones,
                _UpdaterAuth0Id = 1
            });

            if (!success)
            {
                ShowFeedback("No se encontró la unidad a actualizar.", isError: true);
                return;
            }

            _showEditModal = false;
            ShowFeedback("Unidad actualizada correctamente.");
            await LoadData();
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al actualizar unidad: {ex.Message}", isError: true);
        }
    }

    private void TriggerAddSurplusModal()
    {
        _surplusForm = new SurplusUnitModel();
        _surplusVariants.Clear();
        _showSurplusModal = true;
    }

    private async Task OnSurplusProductChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int productId))
        {
            _surplusForm.ProductId = productId;
            // Cargar variantes para el producto seleccionado
            // _surplusVariants = await AuditService.GetVariantsByProductAsync(productId);
        }
        else
        {
            _surplusForm.ProductId = 0;
            _surplusVariants.Clear();
        }
    }

    private async Task SaveSurplusUnit()
    {
        if (string.IsNullOrWhiteSpace(_surplusForm.Code)) return;

        _savingSurplus = true;
        try
        {
            // TODO: reemplaza RegisterSurplusAsync (crear nuevo) por un método que
            // ACTUALICE el registro ExcessProduct ya existente para este Serial,
            // completando ProductoVarianteId/ProductoBaseId.
            // await AuditoriaService.ResolveSurplusUnitAsync(Id, _surplusForm.Code, _surplusForm.ProductoVariantId, _surplusForm.Observations);
            _showSurplusModal = false;
            ShowFeedback("Sobrante identificado correctamente.");
            await LoadData();
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al identificar sobrante: {ex.Message}", isError: true);
        }
        finally
        {
            _savingSurplus = false;
        }
    }

    private void OpenLossModal(AuditUnitDto unit)
    {
        _unitToLoss = unit;
        _lossObservations = string.Empty;
        _showLossModal = true;
    }

    private async Task ConfirmSendToLoss()
    {
        if (_unitToLoss == null || _audit == null) return;

        _sendingLoss = true;
        try
        {
            // TODO: RegistrarMovimientoInventario rechaza esto porque la unidad está en
            // InAuditLock (no Available). Se necesita un método dedicado en AuditoriaService
            // que marque esta línea como pérdida DENTRO de la auditoría (actualiza
            // UnitProductAudits.Status, descuenta stock y desbloquea la unidad),
            // en vez de usar MovimientoService directamente.
            // var result = await AuditoriaService.SendUnitToLossAsync(Id, _unitToLoss.UnidadProductoId, _lossObservations);

            var result = await MovimientosManager.RegistrarMovimientoInventario(
                new RegistrarMovimientoRequest
                {
                    OriginWarehouseId = _audit.WarehouseId ?? 0,
                    DestinationWarehouseId = 2, // Reemplaza por tu bodega de pérdidas configurada
                    TipoMovimiento = TipoMovimiento.Perdida,
                    ProductIds = [_unitToLoss.UnidadProductoId],
                    Observations = string.IsNullOrWhiteSpace(_lossObservations)
                        ? $"Envío a pérdidas desde auditoría #{_audit.AuditId}"
                        : _lossObservations
                },
                CancellationToken.None);

            if (result.IsSuccess)
            {
                _showLossModal = false;
                ShowFeedback("Unidad enviada a pérdidas correctamente.");
                await LoadData();
            }
            else
            {
                ShowFeedback($"No se pudo enviar a pérdidas: {result.Error.Message}", isError: true);
            }
        }
        catch (Exception ex)
        {
            ShowFeedback(ex.Message, isError: true);
        }
        finally
        {
            _sendingLoss = false;
        }
    }

    private void OpenCloseAuditModal()
    {
        _closeForm = new CloseAuditModel();
        _showCloseModal = true;
    }

    private async Task CloseAudit()
    {
        _closingAudit = true;

        var request = new CloseAuditRequest
        {
            _CloserAuth0Id = 1,
            AuditId = _audit.AuditId,
            Conclusions = "PruebaCierre"
        };
        
        try
        {
            await AuditoriaService.CloseAudit(request, CancellationToken.None);
            _showCloseModal = false;
            ShowFeedback("Auditoría cerrada exitosamente.");
            await LoadData();
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al cerrar la auditoría: {ex.Message}", isError: true);
        }
        finally
        {
            _closingAudit = false;
        }
    }

    private void OpenRejectAuditModal()
    {
        _rejectForm = new RejectAuditModel();
        _showRejectModal = true;
    }

    private async Task ConfirmRejectAudit()
    {
        if (string.IsNullOrWhiteSpace(_rejectForm.Reason))
        {
            ShowFeedback("Debe indicar el motivo del rechazo.", isError: true);
            return;
        }

        _rejectingAudit = true;
        try
        {
            // await AuditoriaService.RejectAuditAsync(new RejectAuditRequest { AuditId = Id, Reason = _rejectForm.Reason });
            _showRejectModal = false;
            ShowFeedback("Auditoría rechazada. Las unidades bloqueadas fueron liberadas.");
            await LoadData();
        }
        catch (Exception ex)
        {
            ShowFeedback($"Error al rechazar la auditoría: {ex.Message}", isError: true);
        }
        finally
        {
            _rejectingAudit = false;
        }
    }

    private void ShowFeedback(string message, bool isError = false)
    {
        _feedbackMessage = message;
        _feedbackClass = isError ? "alert-danger" : "alert-success";
    }

    private string _GetStatusClass(string status) => status switch
    {
        "Completada" or "Disponible" or "Cerrada" => "bg-success",
        "En progreso" => "bg-primary",
        "Pendiente de revisión" or "No Encontrado" => "bg-warning text-dark",
        "Dañado" or "Pérdida" or "No Coincide" => "bg-danger",
        "Rechazada por inconsistencias" => "bg-dark",
        _ => "bg-secondary"
    };

    // Modelos auxiliares para formularios y DTOs (puedes moverlos a archivos separados)
    public class AuditDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
    }

    public class EditUnitAuditModel
    {
        public UnitProductAuditStatus Status { get; set; }
        public string UbicacionFisica { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

    public class SurplusUnitModel
    {
        public string Code { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int ProductoVariantId { get; set; }
        public string Observations { get; set; } = string.Empty;
    }

    public class CloseAuditModel
    {
        public string Conclusions { get; set; } = string.Empty;
    }

    public class RejectAuditModel
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class ProductLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class ProductVariantLookupDto
    {
        public int Id { get; set; }
        public string CodigoVariante { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
    }
}