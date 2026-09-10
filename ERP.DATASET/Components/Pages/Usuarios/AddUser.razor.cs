using ERP.DATA.Services.UserService;
using ERP.DATA.Services.VentasService.Stores;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using StoreSummaryDto = ERP.TRAN.CrossLayers.API.Stores.Responses.StoreSummaryDto;

namespace ERP.DATASET.Components.Pages.Usuarios;

public partial class AddUser
{
    [Inject] private UserManager UserService { get; set; } = null!;
    [Inject] private StoresManager StoresManager { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private UserFormModel _model = new();
    private string _confirmPassword = string.Empty;
    private bool _isLoading;
    private string? _errorMessage;

    private static readonly UserRole[] _roles = Enum.GetValues<UserRole>();
    private List<StoreSummaryDto> _tiendas = new();

    private string _roleString
    {
        get => _model.Role.ToString();
        set
        {
            if (Enum.TryParse<UserRole>(value, out var role))
            {
                _model.Role = role;
                if (!RequiereTienda)
                {
                    _model.StoreId = null;
                }
            }
        }
    }

    private bool RequiereTienda =>
        _model.Role == UserRole.Cashier || _model.Role == UserRole.Supervisor;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = new ListStoresRequest(pageNumber: 1, pageSize: -1);
            var tiendas = await StoresManager.List(request, searchTerm: null, CancellationToken.None);
            _tiendas = tiendas.ToList();
        }
        catch (Exception ex)
        {
            _errorMessage = $"No se pudieron cargar las tiendas: {ex.Message}";
        }
    }

    private async Task HandleSubmit()
    {
        _errorMessage = null;

        if (string.IsNullOrWhiteSpace(_model.PrimerNombre))
        {
            _errorMessage = "El primer nombre es obligatorio.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_model.PrimerApellido))
        {
            _errorMessage = "El primer apellido es obligatorio.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_model.Email))
        {
            _errorMessage = "El correo electrónico es obligatorio.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_model.Password))
        {
            _errorMessage = "La contraseña es obligatoria.";
            return;
        }

        if (_model.Password.Length < 6)
        {
            _errorMessage = "La contraseña debe tener al menos 6 caracteres.";
            return;
        }

        if (_model.Password != _confirmPassword)
        {
            _errorMessage = "Las contraseñas no coinciden. Verifíquelas nuevamente.";
            return;
        }

        if (RequiereTienda && _model.StoreId is null)
        {
            _errorMessage = "Debe asignar una tienda para este rol.";
            return;
        }

        _isLoading = true;

        try
        {
            var request = new CrearUsuarioRequest
            {
                PrimerNombre = _model.PrimerNombre.Trim(),
                SegundoNombre = string.IsNullOrWhiteSpace(_model.SegundoNombre) ? null : _model.SegundoNombre.Trim(),
                PrimerApellido = _model.PrimerApellido.Trim(),
                SegundoApellido = string.IsNullOrWhiteSpace(_model.SegundoApellido) ? null : _model.SegundoApellido.Trim(),
                Email = _model.Email.Trim().ToLowerInvariant(),
                Password = _model.Password,
                Role = _model.Role,
                StoreId = RequiereTienda ? _model.StoreId : null
            };

            var result = await UserService.CrearUsuario(request);

            if (result.IsSuccess)
            {
                Navigation.NavigateTo("/usuarios");
            }
            else
            {
                _errorMessage = result.Error.Message;
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Ocurrió un error inesperado: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    public sealed class UserFormModel
    {
        public string PrimerNombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Cashier;
        public int? StoreId { get; set; }
    }
}