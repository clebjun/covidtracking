using BoxTI.Challenge.CovidTracking.Models.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AutoMapper;
using BoxTI.Challenge.CovidTracking.API;
using Microsoft.EntityFrameworkCore;
using BoxTI.Challenge.CovidTracking.API.Models;

namespace BoxTI.Challenge.CovidTracking
{
    public class Startup
    {
        private string _dbContextConnString => Configuration.GetConnectionString("DefaultConnection");
        private string _title => "Desafio Técnico";
        private string _version => "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<Db_Context>(options => options
                .UseSqlServer(_dbContextConnString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo { Title = _title, Version = _version });
                c.CustomSchemaIds(x => x.FullName);
            });

            services.AddAutoMapper(typeof(Startup), typeof(MappingProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var isSwagger = System.Convert.ToBoolean(System.Environment.GetEnvironmentVariable("SWAGGER"));

            app.UseDeveloperExceptionPage();

            if (isSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_title} {_version}"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
