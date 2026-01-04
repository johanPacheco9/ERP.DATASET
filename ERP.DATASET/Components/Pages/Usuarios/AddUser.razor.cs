using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Usuarios;

public partial class AddUser
{
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    protected UserModel User { get; set; } = new();

    protected List<PermisoModel> Permisos { get; set; } = new()
    {
        new("Gestión de Productos", "Crear, editar y eliminar productos"),
        new("Registrar Movimientos", "Entradas, salidas y transferencias"),
        new("Ver Reportes", "Acceso a reportes y estadísticas"),
        new("Gestión de Usuarios", "Administrar cuentas de usuario")
    };

    protected async Task Cerrar()
    {
        await OpenChanged.InvokeAsync(false);
    }

    protected async Task CrearUsuario()
    {
        // TODO: validaciones + llamada a API
        await OpenChanged.InvokeAsync(false);
    }
    public class UserModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Bodega { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }

    public class PermisoModel
    {
        public PermisoModel(string nombre, string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
        }

        public string Nombre { get; }
        public string Descripcion { get; }
        public bool Activo { get; set; }
    }

}
