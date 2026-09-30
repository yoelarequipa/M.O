using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class MovimientoStock
{
    public int Id { get; set; }

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public decimal Cantidad { get; set; }
    public TipoMovimientoStock Tipo { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }

    public TipoDocumentoOrigenStock? TipoDocumentoOrigen { get; set; }
    public int? DocumentoOrigenId { get; set; }
}
