using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.ProductoVarianteService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using Microsoft.AspNetCore.Components;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Movimientos;

public partial class CrearIngresoProductos : ComponentBase
{
    [Inject] public CategoriaService CategoriaService { get; set; } = default!;
    [Inject] public ProductoBaseService productoService { get; set; } = null!;
    [Inject] public ProductVariantService productoVarianteService { get; set; } = null!;
    [Parameter] public EventCallback OnClose { get; set; }

    private int _paso = 1;
    private CrearProductoForm _form = NewForm();
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
            var result = await CategoriaService.List(
                new ListCategoriasRequest { PageNumber = 1, PageSize = 100, OrderBy = "Name" },
                CancellationToken.None);
            _categorias = result?.ToList();
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

        if (_form.CategoriaId <= 0)
            _errores.Add("Debe seleccionar una categoría.");

        if (_errores.Any())
            return;

        try
        {
            _isLoading = true;

            var request = new CreateProductoRequest
            {
                Codigo = _form.Codigo.Trim().ToUpper(),
                Nombre = _form.Nombre.Trim(),
                Descripcion = _form.Descripcion?.Trim(),
                Costo_Unitario = _form.Costo_Unitario,
                Precio_Venta = _form.Precio_Venta,
                PorcentajeIVA = _form.PorcentajeIVA,
                PorcentajeICA = _form.PorcentajeICA,
                ImpuestoEspecifico = _form.ImpuestoEspecifico,
                ArancelImportacion = _form.ArancelImportacion,
                ExentoIVA = _form.ExentoIVA,
                GravadoICA = _form.GravadoICA,
                CodigoTributario = _form.CodigoTributario?.Trim(),
                CategoriaId = _form.CategoriaId,
                ProveedorId = _form.ProveedorId,
                Unidad_Medida = _form.Unidad_Medida,
                Peso = _form.Peso,
                Volumen = _form.Volumen,
                Dimensiones = _form.Dimensiones?.Trim(),
                Imagen_Url = _form.Imagen_Url?.Trim(),
                Notas = _form.Notas?.Trim(),
                Tags = _form.Tags?.Trim(),
                EsPerecedero = _form.Es_Perecedero,
                FechaCaducidad = _form.FechaCaducidad,
                HasVariantes = false
            };

            _productoIdCreado = await productoService.AddProductoAsync(request, CancellationToken.None);
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

            await productoVarianteService.AddProductoVariantes(
                new List<CreateProductoVarianteRequest>
                {
                    new()
                    {
                        ProductoId = _productoIdCreado!.Value,
                        CodigoVariante = _form.Codigo,
                        Atributos = null,
                        PrecioVenta = _form.Precio_Venta,
                        CostoUnitario = _form.Costo_Unitario,
                        CodigoBarras = _form.Codigo
                    }
                },
                CancellationToken.None);

            _skusCreados.Add(_form.Codigo);
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

    private void AgregarVariante()
    {
        _errores.Clear();

        if (string.IsNullOrWhiteSpace(_nuevaVariante.CodigoSufijo))
        {
            _errores.Add("El código de variante es requerido.");
            return;
        }
        if (string.IsNullOrWhiteSpace(_nuevaVariante.Atributos))
        {
            _errores.Add("La descripción de atributos es requerida.");
            return;
        }

        var codigoCompleto = $"{_form.Codigo}-{_nuevaVariante.CodigoSufijo.Trim().ToUpper()}";
        if (_variantesTemp.Any(v => v.CodigoVariante == codigoCompleto))
        {
            _errores.Add($"Ya existe una variante con el código {codigoCompleto}.");
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
            _variantesTemp.RemoveAt(index);
    }

    private async Task GuardarTodasLasVariantes()
    {
        try
        {
            _isLoading = true;
            _errores.Clear();

            if (!_variantesTemp.Any())
            {
                _errores.Add("Debe agregar al menos una variante.");
                return;
            }

            var idsCreados = await productoVarianteService.AddProductoVariantes(
                _variantesTemp.Select(v => new CreateProductoVarianteRequest
                {
                    ProductoId = _productoIdCreado!.Value,
                    CodigoVariante = v.CodigoVariante,
                    Atributos = v.Atributos,
                    PrecioVenta = v.Precio_Venta,
                    CostoUnitario = v.Costo_Unitario,
                    CodigoBarras = v.Codigo_Barras
                }).ToList(),
                CancellationToken.None);

            _skusCreados = _variantesTemp.Select(v => v.CodigoVariante).ToList();
            _totalSkusCreados = idsCreados.Count;
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
        _form = NewForm();
        _errores.Clear();
        _productoIdCreado = null;
        _isLoading = false;
        _variantesTemp.Clear();
        _nuevaVariante = new VarianteTemp();
        _skusCreados.Clear();
        _totalSkusCreados = 0;
    }

    private async Task Cerrar()
    {
        if (OnClose.HasDelegate)
            await OnClose.InvokeAsync();
    }

    private static CrearProductoForm NewForm() => new()
    {
        Unidad_Medida = "UND",
        PorcentajeIVA = 0.19m
    };

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