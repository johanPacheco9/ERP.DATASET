using ERP.DATA.Services.UserService;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Usuarios;

public partial class Update : ComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private UserManager UserManager { get; set; } = null!;

    private EditableUserViewModel? _userModel;
    private bool _isLoading = true;
    private bool _isSaving = false;
    private string? _errorMessage;
    private string? _successMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadUser();
    }

    private async Task LoadUser()
    {
        _isLoading = true;
        _errorMessage = null;

        var result = await UserManager.GetById(Id);

        if (result.IsSuccess && result.Value != null)
        {
            var u = result.Value;
            _userModel = new EditableUserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                PrimerNombre = u.PrimerNombre,
                SegundoNombre = u.SegundoNombre,
                PrimerApellido = u.PrimerApellido,
                SegundoApellido = u.SegundoApellido,
                IsActive = u.IsActive,
                Role = u.Role,
                Password = string.Empty
            };
        }
        else
        {
            _errorMessage = result.Error.Message ?? "No se pudo cargar la información del usuario.";
        }

        _isLoading = false;
    }

    private async Task HandleUpdate()
    {
        if (_userModel == null) return;

        _isSaving = true;
        _errorMessage = null;
        _successMessage = null;

        var request = new UpdateUserRequest(1)
        {
            Id = _userModel.Id,
            Email = _userModel.Email,
            Role = _userModel.Role,
            IsActive = _userModel.IsActive,
            Password = string.IsNullOrWhiteSpace(_userModel.Password) ? null : _userModel.Password
        };

        var result = await UserManager.Update(request);

        if (result.IsSuccess)
        {
            _successMessage = "¡Usuario actualizado exitosamente!";
            _userModel.Password = string.Empty; // Limpiamos el campo de contraseña por seguridad
        }
        else
        {
            _errorMessage = result.Error.Message;
        }

        _isSaving = false;
    }

    private class EditableUserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PrimerNombre { get; set; } = null!;
        public string? SegundoNombre { get; set; }
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public bool IsActive { get; set; }
        public UserRole Role { get; set; }
        public string? Password { get; set; }
    }
}