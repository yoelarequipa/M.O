using MarmolesOeste.Web.Data.Entities;
using MarmolesOeste.Web.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarmolesOeste.Web.Data.Seed;

// Genera ventas históricas ficticias (últimos 12 meses) solo para poder previsualizar
// los gráficos de Facturación con datos. Todo lo que crea queda atado a clientes cuyo
// nombre arranca con "[DEMO] ", así ClearDemoAsync puede borrarlo por completo sin
// tocar nada real. No crea OrdenProduccion, MovimientoStock ni MovimientoCaja a
// propósito, para no ensuciar Producción/Stock/Caja con datos falsos.
public static class DemoVentasSeeder
{
    private const string PrefijoDemo = "[DEMO] ";

    private static readonly string[] NombresDemo =
    [
        "Juan Demo", "María Demo", "Carlos Demo", "Laura Demo", "Roberto Demo",
    ];

    public static async Task<int> SeedAsync(ApplicationDbContext db)
    {
        var materiales = await db.Materiales.ToListAsync();
        var terminaciones = await db.TerminacionesCanto.ToListAsync();
        if (materiales.Count == 0 || terminaciones.Count == 0)
        {
            return 0;
        }

        var clientesDemo = await db.Clientes.Where(c => c.Nombre.StartsWith(PrefijoDemo)).ToListAsync();
        foreach (var nombre in NombresDemo)
        {
            if (clientesDemo.All(c => c.Nombre != PrefijoDemo + nombre))
            {
                var cliente = new Cliente { Nombre = PrefijoDemo + nombre };
                db.Clientes.Add(cliente);
                clientesDemo.Add(cliente);
            }
        }
        await db.SaveChangesAsync();

        var random = new Random();
        var hoyLocal = DateTime.Today;
        var inicioMesActualLocal = new DateTime(hoyLocal.Year, hoyLocal.Month, 1);

        var presupuestosCreados = new List<(Presupuesto Presupuesto, DateTime FechaVentaUtc)>();

        for (var i = 11; i >= 0; i--)
        {
            var mesLocal = inicioMesActualLocal.AddMonths(-i);
            var esMesActual = mesLocal == inicioMesActualLocal;
            var ultimoDia = esMesActual ? hoyLocal.Day : DateTime.DaysInMonth(mesLocal.Year, mesLocal.Month);

            var cantidadVentas = random.Next(2, 7);
            for (var v = 0; v < cantidadVentas; v++)
            {
                var dia = random.Next(1, ultimoDia + 1);
                var hora = random.Next(9, 19);
                var fechaVentaLocal = new DateTime(mesLocal.Year, mesLocal.Month, dia, hora, random.Next(0, 60), 0, DateTimeKind.Local);
                var fechaVentaUtc = fechaVentaLocal.ToUniversalTime();

                var cliente = clientesDemo[random.Next(clientesDemo.Count)];
                var presupuesto = new Presupuesto
                {
                    ClienteId = cliente.Id,
                    Fecha = fechaVentaUtc,
                    DiasValidez = 15,
                    FechaVencimiento = fechaVentaUtc.AddDays(15),
                    Estado = EstadoPresupuesto.Aceptado,
                    IvaPorcentaje = 21m,
                };

                var cantidadPiezas = random.Next(1, 3);
                for (var p = 0; p < cantidadPiezas; p++)
                {
                    var material = materiales[random.Next(materiales.Count)];
                    var terminacion = terminaciones[random.Next(terminaciones.Count)];
                    var largo = Math.Round((decimal)(random.NextDouble() * 2.5 + 1.5), 2);
                    var profundidad = Math.Round((decimal)(random.NextDouble() * 0.3 + 0.6), 2);

                    presupuesto.Piezas.Add(new PresupuestoPieza
                    {
                        Forma = FormaPieza.Recta,
                        Profundidad = profundidad,
                        LlevaZocalo = random.Next(2) == 0,
                        MaterialId = material.Id,
                        NombreMaterialCongelado = material.Nombre,
                        PrecioMaterialCongelado = material.PrecioPorM2,
                        PrecioZocaloCongelado = material.PrecioPorMetroLinealZocalo,
                        TerminacionCantoId = terminacion.Id,
                        NombreTerminacionCongelado = terminacion.Nombre,
                        PrecioTerminacionCongelado = terminacion.PrecioPorMetroLineal,
                        Tramos = { new PresupuestoPiezaTramo { Orden = 1, Largo = largo } },
                    });
                }

                db.Presupuestos.Add(presupuesto);
                presupuestosCreados.Add((presupuesto, fechaVentaUtc));
            }
        }

        await db.SaveChangesAsync();

        foreach (var (presupuesto, fechaVentaUtc) in presupuestosCreados)
        {
            await PresupuestoCalculos.RecalcularTotalesAsync(db, presupuesto.Id);
            db.Ventas.Add(new Venta
            {
                PresupuestoId = presupuesto.Id,
                Fecha = fechaVentaUtc,
                Total = presupuesto.Total,
            });
        }

        await db.SaveChangesAsync();
        return presupuestosCreados.Count;
    }

    public static async Task<int> ClearDemoAsync(ApplicationDbContext db)
    {
        var clientesDemo = await db.Clientes.Where(c => c.Nombre.StartsWith(PrefijoDemo)).ToListAsync();
        if (clientesDemo.Count == 0)
        {
            return 0;
        }

        var clienteIds = clientesDemo.Select(c => c.Id).ToList();
        var presupuestoIds = await db.Presupuestos
            .Where(p => p.ClienteId != null && clienteIds.Contains(p.ClienteId.Value))
            .Select(p => p.Id)
            .ToListAsync();

        var ventas = await db.Ventas.Where(v => presupuestoIds.Contains(v.PresupuestoId)).ToListAsync();
        var cantidadVentas = ventas.Count;
        db.Ventas.RemoveRange(ventas);
        await db.SaveChangesAsync();

        var presupuestos = await db.Presupuestos.Where(p => presupuestoIds.Contains(p.Id)).ToListAsync();
        db.Presupuestos.RemoveRange(presupuestos);
        await db.SaveChangesAsync();

        db.Clientes.RemoveRange(clientesDemo);
        await db.SaveChangesAsync();

        return cantidadVentas;
    }
}
