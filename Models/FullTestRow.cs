namespace PC7866.Models;

/// <summary>Resultado de un paso del test completo manual (una salida activa).</summary>
public sealed class FullTestRow
{
    public int    Output    { get; init; }

    // RAW
    public int    Ain1Raw   { get; set; }
    public int    Ain2Raw   { get; set; }
    public int    Ain3Raw   { get; set; }
    public int    Ain4Raw   { get; set; }

    // Filtrado
    public int    Ain1Filt  { get; set; }
    public int    Ain2Filt  { get; set; }
    public int    Ain3Filt  { get; set; }
    public int    Ain4Filt  { get; set; }

    // Calculados (a partir de filtrado)
    public double Vain      { get; set; }   // Ch1 – Ch2
    public double Ve        { get; set; }   // Ch3 – Ch4
    public double Resistance{ get; set; }   // Ω
    public string Error     { get; set; } = string.Empty;
}
