using ERP.TRAN.CrossLayers.API.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega
{
    public static class BodegasEndpoints
    {

        /// <summary>
        ///     Ruta principal - listar los motivos de requerimientos.
        /// </summary>
        public const string List = "/api/v1/Inventario/Bodegas";

        /// <summary>
        ///     Permite enlistar los registros desde el portal de usuario
        /// </summary>
        //public const string ListUserPortal = $"{List}/user-portal";

        /// <summary>
        ///     Name de la ruta para consultar.
        /// </summary>
        //public const string GetRouteName = "listar motivos para la generación de requerimientos de transacciones";

        /// <summary>
        ///     Ruta para consultar un motivo específico.
        /// </summary>
        public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";

    }
}
