using ClosedXML.Excel;
using MarmolesOeste.Web.Data.Entities;

namespace MarmolesOeste.Web.Pdf;

// Requiere que las Ventas vengan cargadas con Presupuesto.Cliente (Include completo).
public static class FacturacionExcelBuilder
{
    public static byte[] Build(List<Venta> ventas, int anio, string nombreMes)
    {
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add($"Facturación {nombreMes} {anio}");

        hoja.Cell(1, 1).Value = "Fecha";
        hoja.Cell(1, 2).Value = "Cliente";
        hoja.Cell(1, 3).Value = "Presupuesto";
        hoja.Cell(1, 4).Value = "Total";
        hoja.Range(1, 1, 1, 4).Style.Font.Bold = true;

        var fila = 2;
        foreach (var venta in ventas)
        {
            hoja.Cell(fila, 1).Value = venta.Fecha.ToLocalTime().Date;
            hoja.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy";
            hoja.Cell(fila, 2).Value = venta.Presupuesto.Cliente?.Nombre ?? venta.Presupuesto.NombreReferencia ?? "(sin datos)";
            hoja.Cell(fila, 3).Value = $"Nº {venta.Presupuesto.NumeroCorrelativo}";
            hoja.Cell(fila, 4).Value = venta.Total;
            hoja.Cell(fila, 4).Style.NumberFormat.Format = "$ #,##0.00";
            fila++;
        }

        if (ventas.Count > 0)
        {
            hoja.Cell(fila, 3).Value = "Total";
            hoja.Cell(fila, 3).Style.Font.Bold = true;
            hoja.Cell(fila, 4).Value = ventas.Sum(v => v.Total);
            hoja.Cell(fila, 4).Style.NumberFormat.Format = "$ #,##0.00";
            hoja.Cell(fila, 4).Style.Font.Bold = true;
        }

        hoja.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
