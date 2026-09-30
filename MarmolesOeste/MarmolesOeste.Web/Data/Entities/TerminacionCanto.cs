namespace MarmolesOeste.Web.Data.Entities;

public class TerminacionCanto : ISoftDelete, IAuditableActualizacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioPorMetroLineal { get; set; }
    public DateTime? FechaBaja { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    public string? MotivoUltimaActualizacion { get; set; }

    public bool Activo => FechaBaja is null;

    public ICollection<PresupuestoPieza> PresupuestoPiezas { get; set; } = new List<PresupuestoPieza>();
}
