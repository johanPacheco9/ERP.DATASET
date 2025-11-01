namespace ERP.TRAN.CrossLayers.API.Base
{
    /// <summary>
    /// Define los placeholders que se usan en las rutas de los endpoints.
    /// </summary>
    public static class PathLiterals
    {
        /// <summary>
        /// Representa el identificador único de un recurso.
        /// Ejemplo: /api/v1/.../{id}
        /// </summary>
        public const string PrimaryKeyPlaceholder = "{id}";

        /// <summary>
        /// Placeholder opcional para nombres, códigos o cualquier otro identificador secundario.
        /// </summary>
        public const string SecondaryKeyPlaceholder = "{codigo}";

        /// <summary>
        /// Placeholder genérico por si se necesita en rutas personalizadas.
        /// </summary>
        public const string CustomPlaceholder = "{valor}";
    }
}
