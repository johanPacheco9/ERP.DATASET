using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.ClientService;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class CrearVenta
{
    [Inject] private SaleService SaleService { get; set; } = null!;
    [Inject] private ClientService ClientService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private CategoriaService CategoriaService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private List<ClientSummaryDto> _clientes = new();
    private List<WarehouseSummaryDto> _bodegas = new();
    private List<CategoriaDetailDto> _categorias = new();
    private List<BarcodeLookupResultDto> _catalogProducts = new();
    private readonly List<PosCartLine> _cart = new();

    private int _clientId;
    private int _warehouseId;
    private string? _notes;

    // Pistola de códigos de barras
    private string? _barcodeInput;
    private bool _soundEnabled = true;
    private bool _loading = true;
    private bool _saving;
    private string? _error;
    private string? _scanSuccessMessage;

    // Métodos de pago y caja
    private decimal _paymentAmount;
    private PaymentMethod _paymentMethod = PaymentMethod.Cash;

    // Filtros de catálogo visual
    private bool _showCatalogGrid = false;
    private int _selectedCategory = 0;

    // Modales
    private bool _showClientModal = false;
    private string _clientSearchText = "";
    private string _newClientName = "";
    private DniType _newClientDniType = DniType.cc;
    private string _newClientDoc = "";
    private string _newClientEmail = "";
    private string _newClientPhone = "";

    private bool _showReceiptModal = false;
    private SaleDetailDto? _completedSale;

    private ClientSummaryDto? SelectedClient => _clientes.FirstOrDefault(c => c.Id == _clientId);

    private IEnumerable<BarcodeLookupResultDto> FilteredCatalog =>
        _selectedCategory == 0
            ? _catalogProducts
            : _catalogProducts.Where(p => _categorias.Any(c => c.Id == _selectedCategory && c.Nombre == p.Categoria));

    private IEnumerable<ClientSummaryDto> FilteredClients
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_clientSearchText))
                return _clientes;
            var s = _clientSearchText.Trim().ToLower();
            return _clientes.Where(c => c.Name.ToLower().Contains(s) || c.IdentificationNumber.ToLower().Contains(s));
        }
    }

    private decimal CartSubtotal => _cart.Sum(l => l.Subtotal);
    private decimal CartTax => _cart.Sum(l => l.TaxAmount);
    private decimal CartTotal => _cart.Sum(l => l.LineTotal);

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialData();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FocusBarcodeInput();
        }
    }

    private async Task LoadInitialData()
    {
        _loading = true;
        try
        {
            _clientes = await ClientService.ListAsync(CancellationToken.None);
            
            // Garantizar cliente Consumidor Final si la lista está vacía
            if (!_clientes.Any())
            {
                var defaultClient = await ClientService.CreateAsync(new CreateClientRequest
                {
                    Name = "Consumidor Final",
                    DniType = DniType.cc,
                    IdentificationNumber = "222222222222",
                    Email = "consumidorfinal@erp.local",
                    City = "General"
                }, CancellationToken.None);

                if (defaultClient != null)
                {
                    _clientes.Add(defaultClient);
                }
            }

            var bodegas = await WarehouseService.List(new ListWarehousesRequest { PageNumber = 1, PageSize = 50 }, CancellationToken.None);
            _bodegas = bodegas.ToList();

            var categorias = await CategoriaService.List(new ListCategoriasRequest { PageNumber = 1, PageSize = 50 }, CancellationToken.None);
            _categorias = categorias.ToList();

            if (_bodegas.Any())
                _warehouseId = _bodegas[0].Id;

            if (_clientes.Any())
                _clientId = _clientes[0].Id;

            await RefreshCatalog();
        }
        catch (Exception ex)
        {
            _error = $"Error cargando datos iniciales: {ex.Message}";
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task RefreshCatalog()
    {
        if (_warehouseId > 0)
        {
            _catalogProducts = await SaleService.SearchProductsForPosAsync(null, null, _warehouseId, 50, CancellationToken.None);
        }
    }

    private async Task OnWarehouseChanged()
    {
        _cart.Clear();
        await RefreshCatalog();
        await FocusBarcodeInput();
    }

    private void FilterCategory(int categoryId)
    {
        _selectedCategory = categoryId;
        _showCatalogGrid = true;
    }

    private async Task HandleBarcodeKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ProcessBarcodeScan();
        }
        else if (e.Key == "F2")
        {
            await FocusBarcodeInput();
        }
        else if (e.Key == "F4")
        {
            OpenClientModal();
        }
        else if (e.Key == "F9")
        {
            if (_cart.Any() && !_saving)
                await ConfirmarVenta();
        }
        else if (e.Key == "Escape")
        {
            ClearBarcodeInput();
        }
    }

    private async Task ProcessBarcodeScan()
    {
        if (string.IsNullOrWhiteSpace(_barcodeInput))
            return;

        var query = _barcodeInput.Trim();
        _error = null;
        _scanSuccessMessage = null;

        if (_warehouseId <= 0)
        {
            _error = "Seleccione una bodega antes de escanear.";
            await PlayAudioError();
            return;
        }

        var match = await SaleService.LookupProductByBarcodeAsync(query, _warehouseId, CancellationToken.None);

        if (match == null)
        {
            _error = $"No se encontró ningún producto con el código '{query}'.";
            await PlayAudioError();
            _barcodeInput = "";
            await FocusBarcodeInput();
            return;
        }

        // Si se encontró el producto, verificar stock usando ProductoVarianteId
        var existingLine = _cart.FirstOrDefault(c => c.ProductoVarianteId == match.ProductoVarianteId);
        var currentQtyInCart = existingLine?.Quantity ?? 0;

        if (currentQtyInCart + 1 > match.AvailableStock)
        {
            _error = $"Stock insuficiente para '{match.Name}'. Disponible: {match.AvailableStock}, en carrito: {currentQtyInCart}.";
            await PlayAudioError();
            _barcodeInput = "";
            await FocusBarcodeInput();
            return;
        }

        if (existingLine != null)
        {
            existingLine.Quantity += 1;
        }
        else
        {
            _cart.Add(new PosCartLine
            {
                ProductoVarianteId = match.ProductoVarianteId ?? 0,
                ProductName = match.Name,
                ProductCode = match.Code,
                SerialOrSku = match.Serial ?? match.SKU,
                Quantity = 1,
                UnitPrice = match.PrecioVenta,
                TaxRate = match.PorcentajeIVA
            });
        }

        _scanSuccessMessage = $"✓ {match.Name} agregado al carrito.";
        _barcodeInput = "";
        
        if (_paymentMethod == PaymentMethod.Cash && (_paymentAmount == 0 || _paymentAmount < CartTotal))
        {
            _paymentAmount = CartTotal;
        }

        await PlayAudioSuccess();
        await FocusBarcodeInput();
    }

    private void AddProductFromCatalog(BarcodeLookupResultDto prod)
    {
        _error = null;
        _scanSuccessMessage = null;

        var existing = _cart.FirstOrDefault(c => c.ProductoVarianteId == prod.ProductoVarianteId);
        var currentQty = existing?.Quantity ?? 0;

        if (currentQty + 1 > prod.AvailableStock)
        {
            _error = $"Stock insuficiente para '{prod.Name}'. Disponible: {prod.AvailableStock}.";
            return;
        }

        if (existing != null)
        {
            existing.Quantity += 1;
        }
        else
        {
            _cart.Add(new PosCartLine
            {
                ProductoVarianteId = prod.ProductoVarianteId ?? 0,
                ProductName = prod.Name,
                ProductCode = prod.Code,
                SerialOrSku = prod.SKU,
                Quantity = 1,
                UnitPrice = prod.PrecioVenta,
                TaxRate = prod.PorcentajeIVA
            });
        }

        if (_paymentMethod == PaymentMethod.Cash && (_paymentAmount == 0 || _paymentAmount < CartTotal))
        {
            _paymentAmount = CartTotal;
        }
    }

    private void ClearBarcodeInput()
    {
        _barcodeInput = "";
        _error = null;
    }

    private void ClearCart()
    {
        _cart.Clear();
        _paymentAmount = 0;
        _error = null;
    }

    private void DecrementLine(PosCartLine line)
    {
        line.Quantity = Math.Max(1, line.Quantity - 1);
        if (_paymentMethod == PaymentMethod.Cash && _paymentAmount > CartTotal)
            _paymentAmount = CartTotal;
    }

    private async Task IncrementLine(PosCartLine line)
    {
        _error = null;
        // Se ajustó para consultar stock por variante si el servicio lo soporta o manteniendo el método de validación
        var available = await SaleService.GetAvailableStockAsync(line.ProductoVarianteId, _warehouseId, CancellationToken.None);
        if (line.Quantity + 1 > available)
        {
            _error = $"{line.ProductName}: no hay más unidades disponibles en esta bodega.";
            await PlayAudioError();
            return;
        }

        line.Quantity++;
        if (_paymentMethod == PaymentMethod.Cash)
            _paymentAmount = CartTotal;
    }

    private void RemoveLine(PosCartLine line)
    {
        _cart.Remove(line);
        if (_paymentMethod == PaymentMethod.Cash && _paymentAmount > CartTotal)
            _paymentAmount = CartTotal;
    }

    private void SetPaymentMethod(PaymentMethod method)
    {
        _paymentMethod = method;
        if (method == PaymentMethod.Cash)
            _paymentAmount = CartTotal;
        else if (method == PaymentMethod.Credit)
            _paymentAmount = 0;
        else
            _paymentAmount = CartTotal;
    }

    private void SetDefaultClient()
    {
        var defaultClient = _clientes.FirstOrDefault(c => c.IdentificationNumber == "222222222222" || c.Name.Contains("Consumidor Final"));
        if (defaultClient != null)
        {
            _clientId = defaultClient.Id;
        }
        else if (_clientes.Any())
        {
            _clientId = _clientes[0].Id;
        }
    }

    private void OpenClientModal()
    {
        _clientSearchText = "";
        _showClientModal = true;
    }

    private void SelectClient(ClientSummaryDto client)
    {
        _clientId = client.Id;
        _showClientModal = false;
    }

    private async Task CreateAndSelectClient()
    {
        if (string.IsNullOrWhiteSpace(_newClientName) || string.IsNullOrWhiteSpace(_newClientDoc))
        {
            _error = "El nombre y documento del cliente son obligatorios.";
            return;
        }

        try
        {
            var newClient = await ClientService.CreateAsync(new CreateClientRequest
            {
                Name = _newClientName,
                DniType = _newClientDniType,
                IdentificationNumber = _newClientDoc,
                Email = _newClientEmail,
                PhoneNumber = _newClientPhone
            }, CancellationToken.None);

            if (newClient != null)
            {
                _clientes.Insert(0, newClient);
                _clientId = newClient.Id;
                _showClientModal = false;
                _newClientName = "";
                _newClientDoc = "";
                _newClientEmail = "";
                _newClientPhone = "";
            }
        }
        catch (Exception ex)
        {
            _error = $"Error creando cliente: {ex.Message}";
        }
    }

    private async Task ConfirmarVenta()
    {
        if (!_cart.Any())
        {
            _error = "Agregue al menos un producto a la venta.";
            await PlayAudioError();
            return;
        }

        if (_clientId <= 0)
        {
            _error = "Seleccione un cliente antes de confirmar la venta.";
            await PlayAudioError();
            return;
        }

        if (_warehouseId <= 0)
        {
            _error = "Seleccione una bodega de despacho.";
            await PlayAudioError();
            return;
        }

        if (_paymentMethod == PaymentMethod.Cash && _paymentAmount < CartTotal)
        {
            _error = $"El monto recibido ({_paymentAmount:C0}) es menor que el total de la venta ({CartTotal:C0}).";
            await PlayAudioError();
            return;
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
                _CreatorAuth0Id = "cajero-pos",
                PaymentAmount = _paymentMethod == PaymentMethod.Credit ? 0 : Math.Min(_paymentAmount, CartTotal),
                PaymentMethod = _paymentMethod,
                Lines = _cart.Select(c => new SaleLineRequest
                {
                    ProductoVarianteId = c.ProductoVarianteId, 
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    TaxRate = c.TaxRate
                }).ToList()
            };

            _completedSale = await SaleService.CreateAsync(request, CancellationToken.None);
            _showReceiptModal = true;
            _cart.Clear();
            _paymentAmount = 0;
            _notes = null;

            await PlayAudioSuccess();
            await RefreshCatalog();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            await PlayAudioError();
        }
        finally
        {
            _saving = false;
        }
    }
    
    private async Task PrintReceipt()
    {
        await JS.InvokeVoidAsync("erpPos.printReceipt");
    }

    private async Task CloseReceiptModal()
    {
        _showReceiptModal = false;
        _completedSale = null;
        await FocusBarcodeInput();
    }

    private async Task FocusBarcodeInput()
    {
        try
        {
            await JS.InvokeVoidAsync("erpPos.focusInput", "barcode-scanner-input");
        }
        catch {}
    }

    private async Task PlayAudioSuccess()
    {
        if (_soundEnabled)
        {
            try
            {
                await JS.InvokeVoidAsync("erpAudio.playSuccess");
            }
            catch {}
        }
    }

    private async Task PlayAudioError()
    {
        if (_soundEnabled)
        {
            try
            {
                await JS.InvokeVoidAsync("erpAudio.playError");
            }
            catch {}
        }
    }

    private sealed class PosCartLine
    {
        public int ProductoVarianteId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string? SerialOrSku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        
        public decimal Subtotal => UnitPrice * Quantity;
        public decimal TaxAmount => Math.Round(Subtotal * TaxRate, 2);
        public decimal LineTotal => Subtotal + TaxAmount;
    }
}