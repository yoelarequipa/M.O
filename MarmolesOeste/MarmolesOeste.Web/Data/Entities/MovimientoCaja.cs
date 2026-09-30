using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class MovimientoCaja
{
    public int Id { get; set; }

    public TipoMovimientoCaja Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public MedioPago MedioPago { get; set; }
    public DateTime Fecha { get; set; }

    public int? VentaId { get; set; }
    public Venta? Venta { get; set; }
}
