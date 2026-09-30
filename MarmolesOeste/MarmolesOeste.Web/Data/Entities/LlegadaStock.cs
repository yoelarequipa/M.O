namespace MarmolesOeste.Web.Data.Entities;

// Un aviso de "va a llegar un camión con material tal día", cargado a mano por el
// dueño cuando hace un pedido a un proveedor. No reemplaza a MovimientoStock: esto
// es una fecha estimada a futuro, y cuando el camión realmente llega, la entrada de
// stock se sigue cargando como un MovimientoStock normal.
public class LlegadaStock
{
    public int Id { get; set; }
    public DateTime FechaEstimada { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool Recibido { get; set; }
}
