using System.Security.Claims;
using ERP.TRAN.CrossLayers.API.Users.Enums;

namespace ERP.TRAN.CrossLayers.API.Users.Security;

/// <summary>
/// Definiciones estáticas centralizadas de Roles, Permisos y Políticas de acceso para todo el sistema ERP.
/// </summary>
public static class AppPermissions
{
    /// <summary>
    /// Nombres canónicos de los roles en el sistema (coinciden con ClaimTypes.Role y UserRole.ToString()).
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Supervisor = "Supervisor";
        public const string Cashier = "Cashier";
    }

    /// <summary>
    /// Cadenas compuestas de roles para usar directamente en directivas [Authorize(Roles = ...)] y componentes &lt;AuthorizeView Roles="..."&gt;.
    /// </summary>
    public static class Policies
    {
        /// <summary>Acceso universal para todos los usuarios autenticados con rol válido.</summary>
        public const string AllAuthenticated = $"{Roles.Admin},{Roles.Supervisor},{Roles.Cashier}";

        /// <summary>Exclusivo para administradores del sistema (Acceso a configuración, usuarios, tiendas).</summary>
        public const string AdminOnly = Roles.Admin;

        /// <summary>Administradores y Supervisores (Inventario, Compras, Auditorías, Arqueos).</summary>
        public const string SupervisorOrAdmin = $"{Roles.Admin},{Roles.Supervisor}";

        /// <summary>Módulo de Inventario (Productos, Variantes, Kárdex, Categorías, Bodegas, Proveedores, Órdenes de Compra).</summary>
        public const string InventoryManagement = $"{Roles.Admin},{Roles.Supervisor}";

        /// <summary>Módulo de Auditorías de Inventario y Conteos físicos.</summary>
        public const string AuditManagement = $"{Roles.Admin},{Roles.Supervisor}";

        /// <summary>Punto de Venta POS, Historial de Ventas, Clientes y Cartera.</summary>
        public const string PosAndSales = $"{Roles.Admin},{Roles.Supervisor},{Roles.Cashier}";

        /// <summary>Apertura y Cierre de Caja / Turnos de operación.</summary>
        public const string CashRegisterOperation = $"{Roles.Admin},{Roles.Supervisor},{Roles.Cashier}";

        /// <summary>Administración de Cajas Físicas, Resoluciones DIAN y Asignación de Cajeros.</summary>
        public const string CashRegisterAdministration = $"{Roles.Admin},{Roles.Supervisor}";

        /// <summary>Consulta de existencias, catálogo de productos y unidades físicas (Accesible para Cajeros, Supervisores y Admins).</summary>
        public const string ProductAndStockConsultation = $"{Roles.Admin},{Roles.Supervisor},{Roles.Cashier}";

        /// <summary>Gestión y creación de cuentas de usuario.</summary>
        public const string UserManagement = Roles.Admin;

        /// <summary>Gestión de Tiendas y Sucursales físicas.</summary>
        public const string StoreManagement = Roles.Admin;
    }

    #region Métodos de Extensión para ClaimsPrincipal y UserRole

    public static bool IsAdmin(this ClaimsPrincipal? principal) =>
        principal?.IsInRole(Roles.Admin) ?? false;

    public static bool IsSupervisor(this ClaimsPrincipal? principal) =>
        principal?.IsInRole(Roles.Supervisor) ?? false;

    public static bool IsCashier(this ClaimsPrincipal? principal) =>
        principal?.IsInRole(Roles.Cashier) ?? false;

    public static bool CanAccessInventory(this ClaimsPrincipal? principal) =>
        principal.IsAdmin() || principal.IsSupervisor();

    public static bool CanAccessAudits(this ClaimsPrincipal? principal) =>
        principal.IsAdmin() || principal.IsSupervisor();

    public static bool CanAccessPos(this ClaimsPrincipal? principal) =>
        principal != null && (principal.IsAdmin() || principal.IsSupervisor() || principal.IsCashier());

    public static bool CanAdministerCashRegisters(this ClaimsPrincipal? principal) =>
        principal.IsAdmin() || principal.IsSupervisor();

    public static bool CanManageUsers(this ClaimsPrincipal? principal) =>
        principal.IsAdmin();

    public static bool CanManageStores(this ClaimsPrincipal? principal) =>
        principal.IsAdmin();

    public static bool HasAccess(this UserRole role, string policy)
    {
        var roleName = role.ToString();
        var allowedRoles = policy.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return allowedRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    #endregion
}
