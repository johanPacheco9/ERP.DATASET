using ERP.DATA.Services.VentasService.Payments;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class VentaDetalle
{
    [Parameter] public int Id { get; set; }

    [Inject] private SaleService SaleService { get; set; } = null!;
    [Inject] private PaymentsService PaymentService { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private bool _loading = true;
    private bool _savingPayment;
    private bool _showPaymentModal;
    private string? _paymentError;
    private SaleDetailDto? _venta;
    private SalePaymentsSummaryDto? _pagos;
    private AddPaymentForm _paymentForm = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        try
        {
            _venta = await SaleService.GetByIdAsync(Id, CancellationToken.None);
            _pagos = await PaymentService.GetBySaleId(Id, CancellationToken.None);
        }
        catch (Exception)
        {
            _venta = null;
        }
        finally
        {
            _loading = false;
        }
    }

    private void OpenPaymentModal()
    {
        if (_pagos == null) return;

        _paymentError = null;
        _paymentForm = new AddPaymentForm
        {
            Amount = Math.Max(0, _pagos.Balance),
            Method = PaymentMethod.Cash,
            PaidAt = DateTime.Today
        };
        _showPaymentModal = true;
    }

    private async Task SavePayment()
    {
        if (_pagos == null) return;

        _paymentError = null;
        _savingPayment = true;
        try
        {
            _pagos = await PaymentService.AddPayment(new AddPaymentRequest
            {
                SaleId = Id,
                Amount = _paymentForm.Amount,
                Method = _paymentForm.Method,
                PaidAt = _paymentForm.PaidAt,
                Reference = _paymentForm.Reference,
                Notes = _paymentForm.Notes,
                _CreatorId = 1
            }, CancellationToken.None);
            
            _showPaymentModal = false;
            await LoadData();
        }
        catch (Exception ex)
        {
            _paymentError = ex.Message;
        }
        finally
        {
            _savingPayment = false;
        }
    }

    private async Task PrintInvoice()
    {
        await JS.InvokeVoidAsync("erpPos.printReceipt");
    }

    private void EmitirFactusSimulado()
    {
        // Preparado para la llamada a la API de Factus en cuanto el usuario integre sus credenciales
        if (_venta != null)
        {
            // Simular actualización visual
            _venta = _venta with { FactusStatus = "Emitida a DIAN", FactusInvoiceNumber = $"FE-{_venta.Id:D4}" };
        }
    }

    private sealed class AddPaymentForm
    {
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public DateTime PaidAt { get; set; } = DateTime.Today;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
