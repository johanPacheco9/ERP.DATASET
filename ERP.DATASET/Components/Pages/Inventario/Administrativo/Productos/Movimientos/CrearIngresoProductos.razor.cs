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

    // Para el paso 3 - Variantes
    private List<VarianteTemp> _variantesTemp = new();
    private VarianteTemp _nuevaVariante = new();

    // Para el paso 4 - Resumen
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

        // Validaciones
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

            Console.WriteLine($"Producto creado exitosamente con ID: {_productoIdCreado}");

            // Ir al paso 2
            _paso = 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error guardando producto: {ex.Message}");
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

            var variante = new CreateProductoVarianteRequest
            {
                ProductoId = _productoIdCreado!.Value,
                CodigoVariante = _producto.Codigo,
                Atributos = null,
                PrecioVenta = _producto.Precio_Venta,
                CostoUnitario = _producto.Costo_Unitario,
                CodigoBarras = _producto.Codigo
            };

            await productoVarianteService.AddProductoVariante(variante, CancellationToken.None);

            _skusCreados.Add(_producto.Codigo);
            _totalSkusCreados = 1;

            // Ir al paso 4 (éxito)
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
        // Este método se llama cuando el usuario escribe el sufijo
        StateHasChanged();
    }

    private void AgregarVariante()
    {
        _errores.Clear();

        // Validaciones
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

        // Verificar duplicados
        var codigoCompleto = $"{_producto.Codigo}-{_nuevaVariante.CodigoSufijo.Trim().ToUpper()}";
        if (_variantesTemp.Any(v => v.CodigoVariante == codigoCompleto))
        {
            _errores.Add($"Ya existe una variante con el código {codigoCompleto}");
            return;
        }

        // Agregar a la lista temporal
        _variantesTemp.Add(new VarianteTemp
        {
            CodigoVariante = codigoCompleto,
            CodigoSufijo = _nuevaVariante.CodigoSufijo.Trim().ToUpper(),
            Atributos = _nuevaVariante.Atributos.Trim(),
            Precio_Venta = _nuevaVariante.Precio_Venta,
            Costo_Unitario = _nuevaVariante.Costo_Unitario,
            Codigo_Barras = _nuevaVariante.Codigo_Barras?.Trim()
        });

        // Limpiar formulario
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

            // Guardar cada variante
            foreach (var variante in _variantesTemp)
            {
                var request = new CreateProductoVarianteRequest
                {
                    ProductoId = _productoIdCreado!.Value,
                    CodigoVariante = variante.CodigoVariante,
                    Atributos = variante.Atributos,
                    PrecioVenta = variante.Precio_Venta,
                    CostoUnitario = variante.Costo_Unitario,
                    CodigoBarras = variante.Codigo_Barras
                };

                await productoVarianteService.AddProductoVariante(request, CancellationToken.None);
                _skusCreados.Add(variante.CodigoVariante);
            }

            _totalSkusCreados = _skusCreados.Count;

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
        public string Codigo_Barras { get; set; }
    }
}
