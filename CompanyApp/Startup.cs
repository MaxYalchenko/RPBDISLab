using CompanyApp.Data;
using CompanyApp.Middleware;
using CompanyApp.Models;
using CompanyApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CompanyApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddScoped<ICached<PeoplePlan>, CachedPeoplePlan>();
            services.AddScoped<ICached<PeopleFact>, CachedPeopleFact>();
            services.AddScoped<ICached<Subdivision>, CachedSubdivision>();
            services.AddScoped<ICached<SubdivisionFact>, CachedSubdivisionFact>();
            services.AddScoped<ICached<SubdivisionPlan>, CachedSubdivisionPlan>();
            services.AddScoped<ICached<Workpeople>, CachedWorkpeople>();
            


        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context context)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseDbInitializer();
            Initializer.Initialize(context);
            app.UseAuthentication();
            app.UseAuthorization();
            context.GetService<ICached<PeoplePlan>>().AddList("PeoplePlan");
            context.GetService<ICached<PeopleFact>>().AddList("PeopleFact");
            context.GetService<ICached<Subdivision>>().AddList("Subdivision");
            context.GetService<ICached<SubdivisionFact>>().AddList("SubdivisionFact");
            context.GetService<ICached<SubdivisionPlan>>().AddList("SubdivisionPlan");
            context.GetService<ICached<Workpeople>>().AddList("Workpeople");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
