namespace PC7866.Models;

/// <summary>
/// Referencia de producto a ensayar.
/// Cada vez que se modifican los parámetros se crea una nueva Referencia (versionado histórico).
/// </summary>
public class Referencia
{
    public int    Id                { get; set; }
    public bool   BActiva           { get; set; } = true;
    public string ReferenciaNombre  { get; set; } = string.Empty;
    public string Descripcion       { get; set; } = string.Empty;
    public DateTime FechaCreacion   { get; set; } = DateTime.Now;
    public DateTime FechaModificacion { get; set; } = DateTime.Now;

    /// <summary>Imagen en bytes (BLOB). Puede ser null si no se ha configurado.</summary>
    public byte[]? Imagen           { get; set; }

    /// <summary>Parámetros de ensayo asociados (cargados bajo demanda).</summary>
    public List<ParametroEnsayo> Parametros { get; set; } = new();

    public override string ToString() => ReferenciaNombre;
}
