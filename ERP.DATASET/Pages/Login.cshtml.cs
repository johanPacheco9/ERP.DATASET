using ERP.DATA.Services.UserService;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.DATASET.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly UserManager _userManager;

    public LoginModel(UserManager userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public string Usuario { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var request = new IniciarSesionRequest
        {
            Usuario = Usuario,
            Password = Password
        };

        var resultado = await _userManager.IniciarSesion(request);

        if (resultado.IsFailure)
        {
            ErrorMessage = resultado.Error.Message; // AJUSTA: nombre real de la propiedad en tu clase Error
            Password = string.Empty; // no repoblamos la contraseña en el form por seguridad
            return Page();
        }

        return LocalRedirect("/home");
    }
}
