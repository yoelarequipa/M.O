namespace MarmolesOeste.Web.Data.Entities;

public class Cliente : ISoftDelete
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? DireccionObra { get; set; }
    public DateTime? FechaBaja { get; set; }

    public bool Activo => FechaBaja is null;

    public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}
