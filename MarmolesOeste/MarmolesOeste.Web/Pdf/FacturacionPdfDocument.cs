using MarmolesOeste.Web.Data.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MarmolesOeste.Web.Pdf;

// Requiere que las Ventas vengan cargadas con Presupuesto.Cliente (Include completo).
public class FacturacionPdfDocument(List<Venta> ventas, int anio, string nombreMes) : IDocument
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
                column.Item().Text("Facturación").FontSize(14).Bold();
                column.Item().Text($"{nombreMes} {anio}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(20).Column(column =>
        {
            column.Spacing(15);
            column.Item().Element(ComposeVentas);
            column.Item().Element(ComposeTotales);
        });
    }

    private void ComposeVentas(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Fecha");
                    header.Cell().Element(HeaderCell).Text("Cliente");
                    header.Cell().Element(HeaderCell).Text("Presupuesto");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                });

                foreach (var venta in ventas)
                {
                    table.Cell().Element(BodyCell).Text(venta.Fecha.ToLocalTime().ToString("dd/MM/yyyy"));
                    table.Cell().Element(BodyCell).Text(venta.Presupuesto.Cliente?.Nombre ?? venta.Presupuesto.NombreReferencia ?? "(sin datos)");
                    table.Cell().Element(BodyCell).Text($"Nº {venta.Presupuesto.NumeroCorrelativo}");
                    table.Cell().Element(BodyCell).AlignRight().Text($"{venta.Total:C2}");
                }
            });
        });
    }

    private void ComposeTotales(IContainer container)
    {
        container.AlignRight().Width(220).Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Cantidad de ventas");
                row.RelativeItem().AlignRight().Text($"{ventas.Count}");
            });
            column.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Darken1);
            column.Item().Row(row =>
            {
                var estilo = QuestPDF.Infrastructure.TextStyle.Default.Bold().FontSize(12);
                row.RelativeItem().Text("Total facturado").Style(estilo);
                row.RelativeItem().AlignRight().Text($"{ventas.Sum(v => v.Total):C2}").Style(estilo);
            });
        });
    }

    private static IContainer HeaderCell(IContainer container) =>
        container.DefaultTextStyle(x => x.Bold()).PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1);

    private static IContainer BodyCell(IContainer container) =>
        container.PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
}
