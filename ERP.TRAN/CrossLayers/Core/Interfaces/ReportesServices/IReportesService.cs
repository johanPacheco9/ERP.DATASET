using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Reportes.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Responses;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.ReportesServices;

public interface IReportesService
{
    Task<Result<ReporteUtilidadDto>> ObtenerReporteUtilidadAsync(
        ReporteUtilidadRequest request, 
        CancellationToken cancellationToken = default);

    Task<Result<ReporteProductosMasVendidosDto>> ObtenerReporteProductosMasVendidosAsync(
        ReporteProductosMasVendidosRequest request, 
        CancellationToken cancellationToken = default);

    Task<Result<ReporteCajasMasVendedorasDto>> ObtenerReporteCajasMasVendedorasAsync(
        ReporteCajasMasVendedorasRequest request, 
        CancellationToken cancellationToken = default);

    Task<Result<ReporteRotacionInventarioDto>> ObtenerReporteRotacionInventarioAsync(
        ReporteRotacionInventarioRequest request,
        CancellationToken cancellationToken = default);
}
