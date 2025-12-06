using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
public record ReporteDiarioMovimientoDto
(
    DateTime Fecha,
    decimal TotalEntradas,
    decimal TotalSalidas,
    decimal TotalMovimientos,
    string Usuario
);
