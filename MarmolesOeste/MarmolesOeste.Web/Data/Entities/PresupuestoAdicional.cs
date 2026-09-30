namespace MarmolesOeste.Web.Data.Entities;

public class PresupuestoAdicional
{
    public int Id { get; set; }

    public int PresupuestoId { get; set; }
    public Presupuesto Presupuesto { get; set; } = null!;

    public int AdicionalId { get; set; }
    public Adicional Adicional { get; set; } = null!;
    public string NombreAdicionalCongelado { get; set; } = string.Empty;
    public decimal PrecioAdicionalCongelado { get; set; }

    public decimal Cantidad { get; set; }
}
