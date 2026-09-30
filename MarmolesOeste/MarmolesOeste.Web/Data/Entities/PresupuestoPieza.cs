using MarmolesOeste.Web.Data.Enums;

namespace MarmolesOeste.Web.Data.Entities;

public class PresupuestoPieza
{
    public int Id { get; set; }

    public int PresupuestoId { get; set; }
    public Presupuesto Presupuesto { get; set; } = null!;

    public FormaPieza Forma { get; set; }
    public decimal Profundidad { get; set; }
    public bool LlevaZocalo { get; set; }

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public string NombreMaterialCongelado { get; set; } = string.Empty;
    public decimal PrecioMaterialCongelado { get; set; }
    public decimal PrecioZocaloCongelado { get; set; }
    public string ColorMaterialCongelado { get; set; } = "#cfe3d8";

    public int TerminacionCantoId { get; set; }
    public TerminacionCanto TerminacionCanto { get; set; } = null!;
    public string NombreTerminacionCongelado { get; set; } = string.Empty;
    public decimal PrecioTerminacionCongelado { get; set; }

    public ICollection<PresupuestoPiezaTramo> Tramos { get; set; } = new List<PresupuestoPiezaTramo>();
    public ICollection<PresupuestoPiezaCalado> Calados { get; set; } = new List<PresupuestoPiezaCalado>();
}
