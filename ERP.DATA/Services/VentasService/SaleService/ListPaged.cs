using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Literals;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    /// <summary>
    /// Lista ventas con paginación real (Skip/Take en base de datos). Siempre ordena
    /// de la más reciente a la más antigua; el campo OrderBy del request no se usa.
    /// </summary>
    public async Task<PagedList<SaleSummaryDto>> ListPagedAsync(
        ListSalesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var query = context.Sales.AsNoTracking().AsQueryable();

        if (request.WarehouseId is > 0)
        {
            var warehouseId = request.WarehouseId.Value;
            query = query.Where(s => s.WarehouseId == warehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(v =>
                v.SaleNumber.ToLower().Contains(term) ||
                v.Client.Name.ToLower().Contains(term) ||
                v.Client.IdentificationNumber.ToLower().Contains(term) ||
                (v.FactusInvoiceNumber != null && v.FactusInvoiceNumber.ToLower().Contains(term)));
        }

        if (request.MinDate.HasValue)
        {
            var desde = ToUtcDate(request.MinDate.Value);
            query = query.Where(s => s.CreatedAt >= desde);
        }

        if (request.MaxDate.HasValue)
        {
            var max = request.MaxDate.Value;
            var hasta = ToUtcDate(max);

            // Si llega solo la fecha (00:00), se incluye el día completo.
            if (max.TimeOfDay == TimeSpan.Zero)
            {
                var hastaExclusivo = hasta.AddDays(1);
                query = query.Where(s => s.CreatedAt < hastaExclusivo);
            }
            else
            {
                query = query.Where(s => s.CreatedAt <= hasta);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        IQueryable<Sale> paged = query
            .OrderByDescending(s => s.CreatedAt)
            .ThenByDescending(s => s.Id);

        if (request.PageSize != PaginationLiterals.UnlimitedResultsPageSizeFlag)
            paged = paged.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

        // Se proyecta solo lo necesario (sin cargar las líneas completas) y el texto del enum
        // se arma después en memoria, porque GetDisplayName() no se puede traducir a SQL.
        var rows = await paged.Select(s => new
        {
            s.Id,
            s.SaleNumber,
            s.CreatedAt,
            ClientName = s.Client.Name,
            WarehouseName = s.Warehouse.Name,
            s.Subtotal,
            s.TaxAmount,
            s.Total,
            s.Status,
            s.PaymentStatus,
            LinesCount = s.Lines.Count,
            s.FactusStatus,
            s.FactusInvoiceNumber
        }).ToListAsync(cancellationToken);

        var items = rows.Select(r => new SaleSummaryDto(
            r.Id,
            r.SaleNumber,
            r.CreatedAt,
            r.ClientName,
            r.WarehouseName,
            r.Subtotal,
            r.TaxAmount,
            r.Total,
            r.Status.GetDisplayName(),
            r.PaymentStatus.GetDisplayName(),
            r.LinesCount,
            r.FactusStatus,
            r.FactusInvoiceNumber)).ToList();

        return new PagedList<SaleSummaryDto>(items, totalCount, request.PageNumber, request.PageSize);
    }

    // Npgsql solo acepta DateTime en UTC para columnas "timestamp with time zone".
    // Una fecha con Kind sin especificar (típico de un selector de fecha) haría fallar la consulta.
    private static DateTime ToUtcDate(DateTime date) => date.Kind switch
    {
        DateTimeKind.Utc => date,
        DateTimeKind.Local => date.ToUniversalTime(),
        _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
    };
}