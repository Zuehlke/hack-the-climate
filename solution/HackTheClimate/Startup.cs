using CurrieTechnologies.Razor.Clipboard;
using HackTheClimate.Data;
using HackTheClimate.Services;
using HackTheClimate.Services.Search;
using HackTheClimate.Services.Search.Azure;
using HackTheClimate.Services.Similarity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;

namespace HackTheClimate
{
    public class Startup
    {
        private const bool UseFake = false;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<DialogService>();

            services.AddTransient<LegislationService>();

            services.AddSingleton<EntityRecognitionService>();
            services.AddTransient<GraphService>();

            services.AddTransient<AzureSearchFacade>();
            services.AddTransient<DocumentService>();

            services.Configure<AzureSearchConfiguration>(Configuration.GetSection(Constants.Configuration.Search));
            services.Configure<AzureBlobEntitiesConfiguration>(Configuration.GetSection(Constants.Configuration.BlobEntities));

            if (UseFake)
            {
                services.AddTransient<ISimilarityService, FakeSimilarityService>();
                services.AddTransient<IDocumentSearchService, FakeDocumentSearchService>();
            }
            else
            {
                services.AddTransient<IDocumentSearchService, DocumentSearchService>();
                services.AddTransient<ISimilarityService, EntityBasedSimilarityService>();
            }

            services.AddClipboard();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}