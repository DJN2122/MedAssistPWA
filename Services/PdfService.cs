using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using PdfSharpCore;

namespace MedAssistPWA.Services
{
    public class PdfService
    {
        public MemoryStream GeneratePdf(List<string> userMessages, List<string> aiMessages)
        {
            const double marginLeft = 40;
            const double marginTop = 40;
            const double marginRight = 40;
            const double pageWidth = 595;  // A4 width in points
            const double pageHeight = 842; // A4 height in points

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PageSize.A4;

            var graphics = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);
            var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
            var brush = XBrushes.Black;

            // Draw title
            var title = "MedAssist Chat Conversation Report";
            graphics.DrawString(title, titleFont, brush, new XRect(marginLeft, marginTop, pageWidth - marginLeft - marginRight, 0), XStringFormats.TopLeft);

            // Starting Y position after the title
            var yPos = marginTop + 40; // Additional space below title

            // Iterate over messages and print them
            for (int i = 0; i < userMessages.Count; i++)
            {
                string processedUserMessage = InsertLineBreaksAfterPeriod(userMessages[i]);
                string processedAiMessage = InsertLineBreaksAfterPeriod(aiMessages[i]);
                string message = $"\nUser: {processedUserMessage}\n\nAI Medical Assistant: {processedAiMessage}";

                var textSize = graphics.MeasureString(message, font, XStringFormats.TopLeft);
                double wrappedHeight = Math.Ceiling(textSize.Width / (pageWidth - marginLeft - marginRight)) * textSize.Height;

                if (yPos + wrappedHeight > pageHeight - marginTop)
                {
                    // Add new page if content exceeds the current page
                    page = document.AddPage();
                    page.Size = PageSize.A4;
                    graphics = XGraphics.FromPdfPage(page);
                    yPos = marginTop; // Reset Y position for the new page
                }

                // Implement manual text wrapping within the available width
                DrawWrappedText(graphics, message, font, new XRect(marginLeft, yPos, pageWidth - marginLeft - marginRight, wrappedHeight));
                yPos += wrappedHeight + 0; // Additional space between entries
            }

            var stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return stream;
        }

        private void DrawWrappedText(XGraphics graphics, string text, XFont font, XRect rect)
        {
            var lines = text.Split('\n');
            double lineHeight = graphics.MeasureString("Ag", font).Height;
            double y = rect.Y;
            foreach (var line in lines)
            {
                graphics.DrawString(line, font, XBrushes.Black, new XRect(rect.X, y, rect.Width, lineHeight), XStringFormats.TopLeft);
                y += lineHeight;
            }
        }

        private string InsertLineBreaksAfterPeriod(string text)
        {
            return Regex.Replace(text, @"\.(\s+)", ".$1");
        }
    }
}
