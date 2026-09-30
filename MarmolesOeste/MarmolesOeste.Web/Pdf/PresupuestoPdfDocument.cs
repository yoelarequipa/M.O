using MarmolesOeste.Web.Data;
using MarmolesOeste.Web.Data.Entities;
using MarmolesOeste.Web.Diagramas;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MarmolesOeste.Web.Pdf;

// Requiere que el Presupuesto venga cargado con Cliente, Piezas.Tramos y Adicionales
// (Include completo), porque arma todo el documento en un solo recorrido sin volver a
// consultar la base.
public class PresupuestoPdfDocument(Presupuesto presupuesto) : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Mármoles Oeste").FontSize(18).Bold();
                column.Item().Text("La Reja, Buenos Aires");
            });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item().Text($"Presupuesto N° {presupuesto.NumeroCorrelativo}").FontSize(14).Bold();
                column.Item().Text($"Fecha: {presupuesto.Fecha.ToLocalTime():dd/MM/yyyy}");
                column.Item().Text($"Válido hasta: {presupuesto.FechaVencimiento.ToLocalTime():dd/MM/yyyy}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(20).Column(column =>
        {
            column.Spacing(15);
            column.Item().Element(ComposeCliente);
            if (presupuesto.Piezas.Count > 0)
            {
                column.Item().Element(ComposePiezas);
            }
            if (presupuesto.Adicionales.Count > 0)
            {
                column.Item().Element(ComposeAdicionales);
            }
            column.Item().Element(ComposeTotales);
        });
    }

    private void ComposeCliente(IContainer container)
    {
        var cliente = presupuesto.Cliente;
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Item().Text("Cliente").Bold();

            if (cliente is null)
            {
                column.Item().Text(presupuesto.NombreReferencia ?? "A confirmar");
                return;
            }

            column.Item().Text(cliente.Nombre);
            if (!string.IsNullOrWhiteSpace(cliente.Telefono))
            {
                column.Item().Text($"Tel: {cliente.Telefono}");
            }
            if (!string.IsNullOrWhiteSpace(cliente.DireccionObra))
            {
                column.Item().Text($"Obra: {cliente.DireccionObra}");
            }
        });
    }

    private void ComposePiezas(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("Piezas").Bold().FontSize(12);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Forma");
                    header.Cell().Element(HeaderCell).Text("Material");
                    header.Cell().Element(HeaderCell).Text("Terminación");
                    header.Cell().Element(HeaderCell).Text("Área");
                    header.Cell().Element(HeaderCell).Text("Zócalo");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
                });

                foreach (var pieza in presupuesto.Piezas)
                {
                    table.Cell().Element(BodyCell).Text(pieza.Forma.ToString());
                    table.Cell().Element(BodyCell).Text(pieza.NombreMaterialCongelado);
                    table.Cell().Element(BodyCell).Text(pieza.NombreTerminacionCongelado);
                    table.Cell().Element(BodyCell).Text($"{PresupuestoCalculos.AreaM2(pieza):N2} m²");
                    table.Cell().Element(BodyCell).Text(pieza.LlevaZocalo ? "Sí" : "No");
                    table.Cell().Element(BodyCell).AlignRight().Text($"{PresupuestoCalculos.SubtotalPieza(pieza):C2}");
                }
            });

            var numero = 0;
            foreach (var pieza in presupuesto.Piezas)
            {
                numero++;
                column.Item().PaddingTop(10).Element(c => ComposePiezaDiagrama(c, pieza, numero));
            }
        });
    }

    private static void ComposePiezaDiagrama(IContainer container, PresupuestoPieza pieza, int numero)
    {
        var tramos = pieza.Tramos.OrderBy(t => t.Orden).Select(t => t.Largo).ToList();
        var calados = pieza.Calados
            .Select(c => new CaladoInfo(c.Tipo, c.TramoOrden, c.Ancho, c.Fondo, c.DistanciaLateral, c.DistanciaFrontal))
            .ToList();
        var svg = PiezaSvgBuilder.Build(pieza.Forma, tramos, pieza.Profundidad, pieza.LlevaZocalo, pieza.ColorMaterialCongelado, calados);

        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
        {
            row.ConstantItem(180).Height(140).Svg(svg).FitArea();

            row.RelativeItem().PaddingLeft(15).Column(column =>
            {
                column.Item().Text($"Pieza {numero}: {pieza.Forma}").Bold();
                column.Item().Text($"Material: {pieza.NombreMaterialCongelado}");
                column.Item().Text($"Terminación: {pieza.NombreTerminacionCongelado}");
                column.Item().Text($"Área de mesada: {PresupuestoCalculos.AreaM2(pieza):N2} m²");
                if (pieza.LlevaZocalo)
                {
                    column.Item().Text($"Zócalo: {PresupuestoCalculos.LargoTotal(pieza):N2} ml");
                }
            });
        });
    }

    private void ComposeAdicionales(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("Adicionales").Bold().FontSize(12);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Nombre");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Cantidad");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Precio unitario");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
                });

                foreach (var adicional in presupuesto.Adicionales)
                {
                    table.Cell().Element(BodyCell).Text(adicional.NombreAdicionalCongelado);
                    table.Cell().Element(BodyCell).AlignRight().Text($"{adicional.Cantidad:N2}");
                    table.Cell().Element(BodyCell).AlignRight().Text($"{adicional.PrecioAdicionalCongelado:C2}");
                    table.Cell().Element(BodyCell).AlignRight().Text($"{PresupuestoCalculos.SubtotalAdicional(adicional):C2}");
                }
            });
        });
    }

    private void ComposeTotales(IContainer container)
    {
        container.AlignRight().Width(220).Column(column =>
        {
            AgregarFilaTotal(column, "Subtotal", presupuesto.Subtotal.ToString("C2"));
            AgregarFilaTotal(column, $"Descuento ({presupuesto.DescuentoPorcentaje:N0}%)", $"-{presupuesto.Descuento:C2}");
            AgregarFilaTotal(column, $"IVA ({presupuesto.IvaPorcentaje:N0}%)", presupuesto.Iva.ToString("C2"));
            column.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Darken1);
            AgregarFilaTotal(column, "Total", presupuesto.Total.ToString("C2"), destacado: true);
        });
    }

    private static void AgregarFilaTotal(ColumnDescriptor column, string etiqueta, string valor, bool destacado = false)
    {
        column.Item().Row(row =>
        {
            var estilo = destacado
                ? QuestPDF.Infrastructure.TextStyle.Default.Bold().FontSize(12)
                : QuestPDF.Infrastructure.TextStyle.Default;

            row.RelativeItem().Text(etiqueta).Style(estilo);
            row.RelativeItem().AlignRight().Text(valor).Style(estilo);
        });
    }

    private static IContainer HeaderCell(IContainer container) =>
        container.DefaultTextStyle(x => x.Bold()).PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1);

    private static IContainer BodyCell(IContainer container) =>
        container.PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
}
