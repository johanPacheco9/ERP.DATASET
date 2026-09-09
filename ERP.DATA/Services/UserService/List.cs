using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
namespace ERP.DATA.Services.UserService;
public partial class UserManager
{
    public async Task<Result<PagedList<UserDetailDto>>> List(ListUsersRequest request)
    {
        return await Result<PagedList<UserDetailDto>>.TryAsync(async () =>
        {
            var query = context.Usuarios.AsQueryable();
            // 1. Filtrado por término de búsqueda (Search)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.PrimerNombre.ToLower().Contains(searchTerm) ||
                    (u.SegundoNombre != null && u.SegundoNombre.ToLower().Contains(searchTerm)) ||
                    u.PrimerAPellido.ToLower().Contains(searchTerm) ||
                    (u.SegundoAPellido != null && u.SegundoAPellido.ToLower().Contains(searchTerm))
                );
            }

            // 2. Ordenamiento sobre la entidad (antes de proyectar al DTO),
            // para que EF pueda traducir el ORDER BY directamente contra las columnas.
            query = request.OrderBy?.ToLower() switch
            {
                "id" => query.OrderBy(u => u.Id),
                "id desc" => query.OrderByDescending(u => u.Id),
                "primernombre" => query.OrderBy(u => u.PrimerNombre),
                "primernombre desc" => query.OrderByDescending(u => u.PrimerNombre),
                "email" => query.OrderBy(u => u.Email),
                "email desc" => query.OrderByDescending(u => u.Email),
                "role" => query.OrderBy(u => u.Role),
                "role desc" => query.OrderByDescending(u => u.Role),
                _ => query.OrderByDescending(u => u.Id) // Orden por defecto
            };

            // 3. Proyección al DTO, ya con el orden resuelto
            var dtoQuery = query.Select(u => new UserDetailDto(
                u.Id,
                u.Email,
                u.PrimerNombre,
                u.SegundoNombre,
                u.PrimerAPellido,
                u.SegundoAPellido,
                u.IsActive,
                u.Role
            ));

            // 4. Ejecución de la paginación
            var pagedResult = await PagedList<UserDetailDto>.ToPagedListAsync(
                dtoQuery,
                request.PageNumber,
                request.PageSize
            );
            return pagedResult;
        },
        ex => new Error("User.ListFailed", ex.Message));
    }
}