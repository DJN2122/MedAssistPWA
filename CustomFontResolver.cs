using MedAssistPWA.Models;
using PdfSharpCore.Fonts;
using System.IO;
using System.Reflection;

namespace MedAssistPWA
{
    public class CustomFontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        private readonly Fonts _fontLoaded;
        public CustomFontResolver(Fonts FontLoaded)
        {
            _fontLoaded = FontLoaded;
        }

        public byte[] GetFont(string faceName)
        {
            return faceName switch
            {
                "Arial-Bold.ttf" => _fontLoaded.ArialBold,
                "Arial-Italic.ttf" => _fontLoaded.ArialItalic,
                "Arial.ttf" => _fontLoaded.Arial,
                _ => _fontLoaded.Arial,
            };
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {

            if (familyName.Equals("Arial", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold)
                {
                    return new FontResolverInfo("Arial-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Arial-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Arial.ttf");
                }
            }
            return new FontResolverInfo("Arial.ttf");
        }


    }
}
