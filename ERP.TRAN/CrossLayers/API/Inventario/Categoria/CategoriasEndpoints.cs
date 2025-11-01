using ERP.TRAN.CrossLayers.API.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria;
public static class CategoriasEndpoints
{
    public const string List = "/api/v1/Inventario/Categorias";
    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";

}

