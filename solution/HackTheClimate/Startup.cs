using CurrieTechnologies.Razor.Clipboard;
using HackTheClimate.Data;
using HackTheClimate.Services;
using HackTheClimate.Services.Search;
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

            services.AddSingleton<LegislationService>();
            services.AddSingleton<SearchService>();

            services.AddSingleton<EntityRecognitionService>();
            services.AddSingleton<SimilarityService>();
            services.AddSingleton<TopicBasedSimilarityService>();
            services.AddSingleton<TopicService>();

            services.AddSingleton<GraphService>();
            services.AddTransient<DocumentService>();

            services.AddTransient<AzureSearchFacade>();

            services.Configure<AzureSearchConfiguration>(Configuration.GetSection(Constants.Configuration.Search));
            services.Configure<AzureBlobEntitiesConfiguration>(
                Configuration.GetSection(Constants.Configuration.BlobEntities));

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
