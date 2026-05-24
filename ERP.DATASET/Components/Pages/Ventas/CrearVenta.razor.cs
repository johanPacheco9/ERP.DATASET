using ERP.DATA.Services.InventarioService.ProductService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.ClientService;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class CrearVenta
{
    [Inject] private SaleService SaleService { get; set; } = null!;
    [Inject] private ClientService ClientService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private List<ClientSummaryDto> _clientes = new();
    private List<ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses.WarehouseSummaryDto> _bodegas = new();
    private List<ProductoSummaryDto> _productos = new();
    private readonly List<CartLine> _cart = new();

    private int _clientId;
    private int _warehouseId;
    private string? _notes;

    private int _addLineaId;
    private int _addQty = 1;
    private int _addAvailable;
    private bool _loading = true;
    private bool _saving;
    private string? _error;
    private decimal _paymentAmount;
    private PaymentMethod _paymentMethod = PaymentMethod.Cash;

    private int AddAvailableAfterCart =>
        Math.Max(0, _addAvailable - _cart.Where(c => c.LineaProductoId == _addLineaId).Sum(c => c.Quantity));
    
    protected override async Task OnInitializedAsync()
    {
        _clientes = await ClientService.ListAsync(CancellationToken.None);
        var bodegas = await WarehouseService.List(new ListWarehousesRequest { PageNumber = 1, PageSize = 50 }, CancellationToken.None);
        _bodegas = bodegas.ToList();
        var productos = await ProductService.ListAsync(new ListProductRequest(1, 200), CancellationToken.None);
        _productos = productos.ToList();
        if (_bodegas.Any())
            _warehouseId = _bodegas[0].Id;
        if (_clientes.Any())
            _clientId = _clientes[0].Id;
        _loading = false;
    }

    private async Task OnWarehouseChanged()
    {
        _cart.Clear();
        await RefreshAvailable();
    }

    private async Task OnProductChanged()
    {
        await RefreshAvailable();
    }

    private async Task RefreshAvailable()
    {
        if (_warehouseId > 0 && _addLineaId > 0)
            _addAvailable = await SaleService.GetAvailableStockAsync(_addLineaId, _warehouseId, CancellationToken.None);
        else
            _addAvailable = 0;
    }

    private async Task AgregarLinea()
    {
        _error = null;
        if (_addLineaId <= 0 || _warehouseId <= 0)
        {
            _error = "Seleccione bodega y producto.";
            return;
        }

        await RefreshAvailable();
        var alreadyInCart = _cart.Where(c => c.LineaProductoId == _addLineaId).Sum(c => c.Quantity);
        if (_addQty + alreadyInCart > _addAvailable)
        {
            _error = $"Solo hay {_addAvailable} unidades disponibles. Ya tiene {alreadyInCart} en el carrito.";
            return;
        }

        var prod = _productos.First(p => p.Id == _addLineaId);
        var existing = _cart.FirstOrDefault(c => c.LineaProductoId == _addLineaId);
        if (existing != null)
        {
            existing.Quantity += _addQty;
        }
        else
        {
            _cart.Add(new CartLine
            {
                LineaProductoId = _addLineaId,
                ProductName = prod.Nombre,
                Quantity = _addQty,
                UnitPrice = prod.PrecioVenta
            });
        }

        _addQty = 1;
        if (_paymentMethod == PaymentMethod.Cash && _paymentAmount == 0)
            _paymentAmount = CartTotal;
        await RefreshAvailable();
    }

    private void QuitarLinea(CartLine line)
    {
        _cart.Remove(line);
        if (_paymentAmount > CartTotal)
            _paymentAmount = CartTotal;
    }

    private void DecrementLine(CartLine line)
    {
        line.Quantity = Math.Max(1, line.Quantity - 1);
        if (_paymentAmount > CartTotal)
            _paymentAmount = CartTotal;
    }

    private async Task IncrementLine(CartLine line)
    {
        _error = null;
        var available = await SaleService.GetAvailableStockAsync(line.LineaProductoId, _warehouseId, CancellationToken.None);
        if (line.Quantity + 1 > available)
        {
            _error = $"{line.ProductName}: no hay mas unidades disponibles en esta bodega.";
            return;
        }

        line.Quantity++;
        if (_paymentMethod == PaymentMethod.Cash)
            _paymentAmount = CartTotal;
    }

    private decimal CartTotal => _cart.Sum(l => l.UnitPrice * l.Quantity);

    private void PagarTotal() => _paymentAmount = CartTotal;

    private async Task ConfirmarVenta()
    {
        if (!_cart.Any())
        {
            _error = "Agregue al menos un producto.";
            return;
        }

        if (_clientId <= 0)
        {
            _error = "Seleccione un cliente antes de confirmar la venta.";
            return;
        }

        if (_warehouseId <= 0)
        {
            _error = "Seleccione una bodega de despacho.";
            return;
        }

        if (_paymentAmount < 0)
        {
            _error = "El monto pagado no puede ser negativo.";
            return;
        }

        foreach (var line in _cart)
        {
            var available = await SaleService.GetAvailableStockAsync(line.LineaProductoId, _warehouseId, CancellationToken.None);
            if (line.Quantity > available)
            {
                _error = $"{line.ProductName}: stock insuficiente. Disponible: {available}, solicitado: {line.Quantity}.";
                return;
            }
        }

        _saving = true;
        _error = null;
        try
        {
            var request = new CreateSaleRequest
            {
                ClientId = _clientId,
                WarehouseId = _warehouseId,
                StoreId = 1,
                Notes = _notes,
                _CreatorAuth0Id = "system",
                PaymentAmount = _paymentAmount,
                PaymentMethod = _paymentMethod,
                Lines = _cart.Select(c => new SaleLineRequest
                {
                    LineaProductoId = c.LineaProductoId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice
                }).ToList()
            };

            var sale = await SaleService.CreateAsync(request, CancellationToken.None);
            Navigation.NavigateTo($"/ventas/{sale.Id}");
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

    private sealed class CartLine
    {
        public int LineaProductoId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
