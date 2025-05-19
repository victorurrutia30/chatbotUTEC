using ChatbotUTEC.Dialogs;
using ChatbotUTEC.Services;        // <-- Asegúrate de este using
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

namespace EchoBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient()
                    .AddControllers()
                    .AddNewtonsoftJson();

            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<CLUPredictor>();
            services.AddSingleton<DatabaseHelper>();
            services.AddSingleton<ReportService>();      // <-- Registro de ReportService
            services.AddTransient<IBot, MainDialog>();
            services.AddHttpClient();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseDefaultFiles()
               .UseStaticFiles()
               .UseWebSockets()
               .UseRouting()
               .UseAuthorization();

            app.UseEndpoints(endpoints =>
    {
                       // Ruta por defecto a AdminController.Index
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Admin}/{action=Index}/{id?}");
                       // tus endpoints API (ReportsController, BotController…)
                endpoints.MapControllers();
                    });
        }
    }
}
