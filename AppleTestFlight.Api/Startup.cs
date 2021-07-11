using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AppleTestFlight.Api.Filters;
using Microsoft.AspNetCore.Http;

namespace AppleTestFlight.Api
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //限制流量过滤器
            services.AddControllers(o =>
            {
                o.Filters.Add(new LimitFilter(Configuration.GetSection("AppleTestFlightConfig:LimitAccessorTime").Value));
            });
            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AppleTestFlight.Api", Version = "v1" });
            });

            //配置跨域
            services.AddCors(options =>
                options.AddPolicy("CustomCorsRules",
                  p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
            );
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppleTestFlight.Api v1"));
            }

            app.UseRouting();

            app.UseCors("CustomCorsRules");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
