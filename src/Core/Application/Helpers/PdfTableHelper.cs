using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Application.Helpers
{
    public class PdfTableHelper(PdfTableStyle pdfTableStyle = null)
    {
        public PdfTableStyle PdfTableStyle { get; } = pdfTableStyle ?? new PdfTableStyle();

        public void CreateTitleDocument(Document doc, string title)
        {
            Paragraph titleDocument = new Paragraph(title)
                .SetFont(PdfTableStyle.FontTitleDocument)
                .SetFontSize(PdfTableStyle.FontSizeTitle)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);

            doc.Add(titleDocument);
        }

        public void CreateTable(Document doc, string title, Dictionary<string, string> values)
        {
            Table table = new Table(UnitValue.CreatePercentArray(PdfTableStyle.NumColumns))
                .SetWidth(UnitValue.CreatePercentValue(PdfTableStyle.WidthPercentage))
                .SetHorizontalAlignment(PdfTableStyle.PositionTable);

            Cell titleCell = CreateTitle(title);
            table.AddHeaderCell(titleCell);

            AddColumnsToTable(table, values);

            doc.Add(table);
            doc.Add(new Paragraph(" ").SetMarginBottom(5));
        }

        private Cell CreateTitle(string title)
        {
            Cell cell = new Cell(1, PdfTableStyle.NumColumns)
                .Add(
                    new Paragraph(title)
                        .SetFont(PdfTableStyle.FontTitle)
                        .SetFontSize(PdfTableStyle.FontSizeHeaderTable)
                )
                .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                .SetTextAlignment(PdfTableStyle.TextAlignmentTitleDocument)
                .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

            return cell;
        }

        private void AddColumnsToTable(Table table, Dictionary<string, string> columns)
        {
            foreach (var value in columns)
            {
                var keyColumn = new Cell()
                    .Add(
                        new Paragraph(value.Key)
                            .SetFont(PdfTableStyle.FontColumns)
                            .SetFontSize(PdfTableStyle.FontSizeColumns)
                            .SetTextAlignment(PdfTableStyle.TextAlignmentColumnKey)
                    )
                    .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

                table.AddCell(keyColumn);

                var valueColumn = new Cell()
                    .Add(
                        new Paragraph(value.Value)
                            .SetFont(PdfTableStyle.FontColumns)
                            .SetFontSize(PdfTableStyle.FontSizeColumns)
                            .SetTextAlignment(PdfTableStyle.TextAlignmentColumnValue)
                    )
                    .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

                table.AddCell(valueColumn);
            }
        }

        public void CreateSingleColumnTable(Document doc, List<string> items)
        {
            Table table = new Table(1)
                .SetWidth(UnitValue.CreatePercentValue(PdfTableStyle.WidthPercentage))
                .SetHorizontalAlignment(PdfTableStyle.PositionTable);

            foreach (var item in items)
            {
                var cell = new Cell()
                    .Add(
                        new Paragraph(item)
                            .SetFont(PdfTableStyle.FontColumns)
                            .SetFontSize(PdfTableStyle.FontSizeColumns)
                            .SetTextAlignment(PdfTableStyle.TextAlignmentColumnValue)
                    )
                    .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                    .SetPadding(5)
                    .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

                table.AddCell(cell);
            }

            doc.Add(table);
            doc.Add(new Paragraph(" ").SetMarginBottom(5));
        }
    }

    public class PdfTableStyle(
        PdfFont fontTitle = null,
        PdfFont fontColumns = null,
        PdfFont fontTitleDocument = null,
        sbyte? fontSizeTitle = null,
        sbyte? fontSizeHeaderTable = null,
        sbyte? fontSizeColumns = null,
        HorizontalAlignment? positionTable = null,
        Color backgroundColor = null,
        float? borderWidth = null,
        sbyte? widthPercentage = null,
        sbyte? numColumns = null,
        TextAlignment? textAlignmentTitleDocument = null,
        TextAlignment? textAlignmentColumnKey = null,
        TextAlignment? textAlignmentColumnValue = null
    )
    {
        public PdfFont FontTitle { get; set; } =
            fontTitle ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        public PdfFont FontColumns { get; set; } =
            fontColumns ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        public PdfFont FontTitleDocument { get; set; } =
            fontTitleDocument ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        public sbyte FontSizeTitle { get; set; } = fontSizeTitle ?? 16;
        public sbyte FontSizeHeaderTable { get; set; } = fontSizeHeaderTable ?? 12;
        public sbyte FontSizeColumns { get; set; } = fontSizeColumns ?? 11;

        public HorizontalAlignment PositionTable { get; set; } = positionTable ?? HorizontalAlignment.CENTER;
        public Color BackgroundColor { get; set; } = backgroundColor ?? new DeviceRgb(249, 249, 249);
        public float BorderWidth { get; set; } = borderWidth ?? 1.5f;
        public sbyte WidthPercentage { get; set; } = widthPercentage ?? 80;
        public sbyte NumColumns { get; set; } = numColumns ?? 2;


        public TextAlignment TextAlignmentTitleDocument { get; set; } =
            textAlignmentTitleDocument ?? TextAlignment.CENTER;
        public TextAlignment TextAlignmentColumnKey { get; set; } =
            textAlignmentColumnKey ?? TextAlignment.CENTER;

        public TextAlignment TextAlignmentColumnValue { get; set; } =
          textAlignmentColumnValue ?? TextAlignment.CENTER;
    }
}
