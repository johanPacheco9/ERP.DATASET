using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Movimientos;

public partial class CrearIngresoProductos : ComponentBase
{
    [Inject] public ICategoriaService CategoriaService { get; set; } = default!;
    [Inject] public IProductoService productoService { get; set; } = null!;
    [Inject] public IProductoVarianteService productoVarianteService { get; set; } = null!;
    [Parameter] public EventCallback OnClose { get; set; }

    private int _paso = 1;
    private bool? _quiereVariantes = null;
    private CreateProductoRequest _producto = new();
    private List<CategoriaDetailDto>? _categorias;
    private bool _isLoading = false;
    private List<string> _errores = new();
    private int? _productoIdCreado = null;

    private List<VarianteTemp> _variantesTemp = new();
    private VarianteTemp _nuevaVariante = new();

    private List<string> _skusCreados = new();
    private int _totalSkusCreados = 0;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _isLoading = true;
            await LoadCategoriasAsync();
        }
        catch (Exception ex)
        {
            _errores.Add($"Error inicializando: {ex.Message}");
            Console.WriteLine($"Error inicializando: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LoadCategoriasAsync()
    {
        try
        {
            var request = new ListCategoriasRequest
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Nombre"
            };
            var result = await CategoriaService.ListAsync(request, CancellationToken.None);
            _categorias = result?.ToList();
            Console.WriteLine($"Categorías cargadas: {_categorias?.Count ?? 0}");
        }
        catch (Exception ex)
        {
            _errores.Add($"Error cargando categorías: {ex.Message}");
            _categorias = null;
        }
    }

    private async Task CrearProducto()
    {
        _errores.Clear();

        if (string.IsNullOrWhiteSpace(_producto.Codigo))
            _errores.Add("El código es requerido");
        if (string.IsNullOrWhiteSpace(_producto.Nombre))
            _errores.Add("El nombre es requerido");
        if (_producto.CategoriaId == 0)
            _errores.Add("Debe seleccionar una categoría");

        if (_errores.Any())
        {
            Console.WriteLine($"Errores de validación: {string.Join(", ", _errores)}");
            return;
        }

        try
        {
            _isLoading = true;
            _producto.hasVariantes = false;

            _productoIdCreado = await productoService.AddProductoAsync(_producto, CancellationToken.None);
            _paso = 2;
        }
        catch (Exception ex)
        {
            _errores.Add($"Error del servidor: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task CrearVarianteUnica()
    {
        try
        {
            _isLoading = true;
            _errores.Clear();
            var variantes = new List<CreateProductoVarianteRequest>
            {
                new CreateProductoVarianteRequest
                {
                    ProductoId = _productoIdCreado!.Value,
                    CodigoVariante = _producto.Codigo,
                    Atributos = null,
                    PrecioVenta = _producto.Precio_Venta,
                    CostoUnitario = _producto.Costo_Unitario,
                    CodigoBarras = _producto.Codigo
                }
            };

            var ids = await productoVarianteService.AddProductoVariantes(
                variantes,
                CancellationToken.None
            );

            _skusCreados.Add(_producto.Codigo);
            _totalSkusCreados = 1;

            _paso = 4;
        }
        catch (Exception ex)
        {
            _errores.Add($"Error creando SKU: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void IrADefinirVariantes()
    {
        _paso = 3;
        _variantesTemp.Clear();
        _nuevaVariante = new VarianteTemp();
    }

    private void GenerarCodigoCompleto()
    {
        StateHasChanged();
    }

    private void AgregarVariante()
    {
        _errores.Clear();

        if (string.IsNullOrWhiteSpace(_nuevaVariante.CodigoSufijo))
        {
            _errores.Add("El código de variante es requerido");
            return;
        }

        if (string.IsNullOrWhiteSpace(_nuevaVariante.Atributos))
        {
            _errores.Add("La descripción de atributos es requerida");
            return;
        }

        var codigoCompleto = $"{_producto.Codigo}-{_nuevaVariante.CodigoSufijo.Trim().ToUpper()}";
        if (_variantesTemp.Any(v => v.CodigoVariante == codigoCompleto))
        {
            _errores.Add($"Ya existe una variante con el código {codigoCompleto}");
            return;
        }
        _variantesTemp.Add(new VarianteTemp
        {
            CodigoVariante = codigoCompleto,
            CodigoSufijo = _nuevaVariante.CodigoSufijo.Trim().ToUpper(),
            Atributos = _nuevaVariante.Atributos.Trim(),
            Precio_Venta = _nuevaVariante.Precio_Venta,
            Costo_Unitario = _nuevaVariante.Costo_Unitario,
            Codigo_Barras = _nuevaVariante.Codigo_Barras?.Trim()
        });
        _nuevaVariante = new VarianteTemp();
    }

    private void EliminarVariante(int index)
    {
        if (index >= 0 && index < _variantesTemp.Count)
        {
            _variantesTemp.RemoveAt(index);
        }
    }

    private async Task GuardarTodasLasVariantes()
    {
        try
        {
            _isLoading = true;
            _errores.Clear();

            if (!_variantesTemp.Any())
            {
                _errores.Add("Debe agregar al menos una variante");
                return;
            }
            var variantesRequests = _variantesTemp.Select(variante =>
                new CreateProductoVarianteRequest
                {
                    ProductoId = _productoIdCreado!.Value,
                    CodigoVariante = variante.CodigoVariante,
                    Atributos = variante.Atributos,
                    PrecioVenta = variante.Precio_Venta,
                    CostoUnitario = variante.Costo_Unitario,
                    CodigoBarras = variante.Codigo_Barras
                }
            ).ToList();

            var idsCreados = await productoVarianteService.AddProductoVariantes(
                variantesRequests,
                CancellationToken.None
            );

            _skusCreados = _variantesTemp.Select(v => v.CodigoVariante).ToList();
            _totalSkusCreados = idsCreados.Count;

            Console.WriteLine($"Variantes creadas: {_totalSkusCreados}");

            // Ir al paso 4 (éxito)
            _paso = 4;
        }
        catch (Exception ex)
        {
            _errores.Add($"Error guardando variantes: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void Reiniciar()
    {
        _paso = 1;
        _quiereVariantes = null;
        _producto = new CreateProductoRequest();
        _errores.Clear();
        _productoIdCreado = null;
        _isLoading = false;
        _variantesTemp.Clear();
        _nuevaVariante = new VarianteTemp();
        _skusCreados.Clear();
        _totalSkusCreados = 0;
    }

    private class VarianteTemp
    {
        public string CodigoVariante { get; set; } = string.Empty;
        public string CodigoSufijo { get; set; } = string.Empty;
        public string Atributos { get; set; } = string.Empty;
        public decimal Precio_Venta { get; set; }
        public decimal Costo_Unitario { get; set; }
        public string? Codigo_Barras { get; set; }
    }
}
