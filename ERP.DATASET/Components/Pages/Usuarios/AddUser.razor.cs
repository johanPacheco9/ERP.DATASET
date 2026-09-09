using ERP.DATA.Services.UserService;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Usuarios;

public partial class AddUser
{
    [Inject] private UserManager UserService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private UserFormModel _model = new();
    private string _confirmPassword = string.Empty;
    private bool _isLoading;
    private string? _errorMessage;

    private string _roleString
    {
        get => _model.Role.ToString();
        set
        {
            if (Enum.TryParse<UserRole>(value, out var role))
            {
                _model.Role = role;
            }
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
                Role = _model.Role
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
    }
}
