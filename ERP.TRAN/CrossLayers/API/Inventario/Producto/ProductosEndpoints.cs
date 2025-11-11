using ERP.TRAN.CrossLayers.API.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto;
public static class ProductosEndpoints
{
    public const string List = "/api/v1/inventario/Productos";

    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";
}

