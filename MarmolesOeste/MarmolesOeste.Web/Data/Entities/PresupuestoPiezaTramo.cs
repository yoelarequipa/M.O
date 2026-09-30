namespace MarmolesOeste.Web.Data.Entities;

// Un tramo de una pieza (una pieza recta tiene 1, una L tiene 2, una U tiene 3).
// La profundidad es común a toda la pieza y vive en PresupuestoPieza; acá solo
// varía el largo de cada segmento.
public class PresupuestoPiezaTramo
{
    public int Id { get; set; }

    public int PresupuestoPiezaId { get; set; }
    public PresupuestoPieza PresupuestoPieza { get; set; } = null!;

    public int Orden { get; set; }
    public decimal Largo { get; set; }
}
