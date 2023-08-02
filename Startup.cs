using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using API.Services;
using API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace API;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["KeyJwt"] ?? string.Empty)),
                ClockSkew = TimeSpan.Zero
            });

        serviceCollection.AddControllers(options =>
            {
                options.Conventions.Add(new SwaggerVersion());
            }).AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddNewtonsoftJson();

        serviceCollection.AddControllers();
        
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "WebAPIAuthors", 
                Version = "v1",
                Description = "The best API"
            });
            c.SwaggerDoc("v2", new OpenApiInfo()
            {
                Title = "WebAPIAuthors", 
                Version = "v2",
                Description = "The best API"
            });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            var fileXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var route = Path.Combine(AppContext.BaseDirectory, fileXml);
            c.IncludeXmlComments(route);
        });
        serviceCollection.AddAutoMapper(typeof(Startup));

        serviceCollection.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdmin", policy => policy.RequireClaim("IsAdmin"));
            options.AddPolicy("IsSeller", policy => policy.RequireClaim("IsSeller"));
        });

        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[]{"TotalRegisters"});
            });
        });

        serviceCollection.AddDataProtection();

        serviceCollection.AddTransient<HashService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAuthors v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAuthors v2");
            });
        }

        app.UseHttpsRedirection();

        app.UseCors();
            
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
        });
    }
}