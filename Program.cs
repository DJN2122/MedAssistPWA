using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MedAssistPWA.Services;
using System;
using PdfSharpCore.Fonts;

namespace MedAssistPWA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(new OpenAiService("INSERT OPENAI API KEY HERE"));
            builder.Services.AddSingleton<PdfService>();
            builder.Services.AddScoped<FontServices>();
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
