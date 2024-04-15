using MedAssistPWA.Models;
namespace MedAssistPWA.Services
{
    public sealed class FontServices
    {
        private readonly HttpClient _httpClient;

        public FontServices(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<Fonts> LoadFonts()
        {
            Fonts fonts = new()
            {
                Arial = await GetFontData("Arial.ttf"),
                ArialBold = await GetFontData("Arial-Bold.ttf"),
                ArialItalic = await GetFontData("Arial-Italic.ttf")
            };
            return fonts;
        }

        private async Task<byte[]> GetFontData(string name)
        {
            var sourceStream = await _httpClient.GetStreamAsync($"fonts/{name}");

            using MemoryStream memoryStream = new();

            sourceStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
