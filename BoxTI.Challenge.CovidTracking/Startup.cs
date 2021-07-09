using BoxTI.Challenge.CovidTracking.Models.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using BoxTI.Challenge.CovidTracking.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BoxTI.Challenge.CovidTracking
{
    public class Startup
    {
        private string _dbContextConnString => Configuration.GetConnectionString("DefaultConnection");
        private string _title => "Desafio Técnico BOXTI";
        private string _version => "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.ClaimsIssuer = "myapi.com";
                options.Audience = "myapi.com";
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("antoniocleberdossantosjunior")),
                    ValidateLifetime = true,
                    ValidIssuer = "myapi.com",
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true
                };
                options.RequireHttpsMetadata = false;
            });

            services.AddDbContext<Db_Context>(options => options
                .UseSqlServer(_dbContextConnString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo { Title = _title, Version = _version });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Main API v1.0", Version = "v1.0" });
                
            });

            services.AddAutoMapper(typeof(Startup), typeof(MappingProfile));
        }

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
