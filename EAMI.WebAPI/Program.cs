using EAMI.DataEngine;
using EAMI.Filters;
using EAMI.Respository.EAMI_Dental_Repository;
using EAMI.Respository.EAMI_MC_Repository;
using EAMI.Respository.EAMI_RX_Repository;
using EAMI.RuleEngine;
using EAMI.WebApi.Endpoints;
using EAMI.WebAPI.Common;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace EAMI.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateWebHostBuilder(args).Build().Run();
            var builder = WebApplication.CreateBuilder(args);

            string[] origins = builder.Configuration["AllowedCorsHosts"]?.Split(';') ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                     builder.WithOrigins(origins)
                    //builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EAMI WebAPI", Version = "v1" });
                c.OperationFilter<CustomHeaderOperationFilter>(); // Add the custom header operation filter
                c.AddSecurityDefinition("Windows", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "negotiate",
                   // In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Windows Authentication"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Windows"
                           }
                       },
                       new string[] {}
                   }
                });
            });

            builder.Services.AddSingleton<IConfiguration>(provider =>
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                return configurationBuilder.Build();
            });

            // ...Add database context
            var configuration = builder.Configuration;
            var pharmacyDbConnectionString = configuration.GetConnectionString("PharmacyDB");
            var dentalDbConnectionString = configuration.GetConnectionString("DentalDB");
            var managedDbConnectionString = configuration.GetConnectionString("ManagedCareDB");

            if (pharmacyDbConnectionString == null || dentalDbConnectionString == null || managedDbConnectionString == null)
            {
                throw new InvalidOperationException("One or more connection strings are not configured.");
            }
            // Add database contexts
            builder.Services.AddDbContext<EAMI_RX>(options =>
                options.UseSqlServer(pharmacyDbConnectionString));

            builder.Services.AddDbContext<EAMI_Dental>(options =>
                options.UseSqlServer(dentalDbConnectionString));

            builder.Services.AddDbContext<EAMI_MC>(options =>
                options.UseSqlServer(managedDbConnectionString));

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<MapperConfigSetup.MapperConfig>();

            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:7777/");

            //if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            //{
            //    //These headers are handling the "pre-flight" OPTIONS call sent by the browser
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "content-type");
            //    HttpContext.Current.Response.End();
            //}           

            // Register dependencies in the dependency injection container...            
            builder.Services.AddTransient<IProgramChoiceRE, ProgramChoiceRE>();
            builder.Services.AddTransient<IProgramChoiceDE, ProgramChoiceDE>();
            builder.Services.AddTransient<IAuthenticationDE, AuthenticationDE>();
            //builder.Services.AddTransient<IPaymentProcessingRE, PaymentProcessingRE>();
            builder.Services.AddTransient<IUserProfileRE, UserProfileRE>();
            builder.Services.AddTransient<IUserProfileDE, UserProfileDE>();
            builder.Services.AddTransient<IUserAuthorizeRE, UserAuthorizeRE>();
            builder.Services.AddTransient<IUserAuthorizeDE, UserAuthorizeDE>();
            builder.Services.AddTransient<EAMIAuthorizeAttribute>();
           
            builder.Services.AddScoped<BaseMiddlewareServiceDE>();
            builder.Services.AddScoped<DataAccessBase>();

            builder.Services.AddControllersWithViews();
            // Add authentication services
            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();

            // builder.Services.AddAuthentication(WindowsIdentity.GetCurrent().Name).AddNegotiate();
            //.AddMicrosoftAccount(config =>
            //{
            //    config.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            //    config.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            //    config.SignInScheme = WindowsIdentity.DefaultNameClaimType;
            //    config.TokenValidationParameters = TokenValidationParameters;
            //});
            
            //builder.Services.AddControllersWithViews(options =>
            //{
            //    options.Filters.Add<EAMIAuthorizeAttribute>();
            //});
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EAMI v1"));
            //}

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            ///	Register Middleware...
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            //    if (context.Request.Method == "OPTIONS")
            //    {
            //        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            //        context.Response.Headers.Append("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            //        //context.Response.Headers.Append("Access-Control-Allow-Headers", "*"); // content-type");
            //        context.Response.Headers.Append("Access-Control-Allow-Credentials", "false");
            //        context.Response.StatusCode = 204; // No Content
            //        return;
            //    }
            //    await next();
            //});

           // LocationEndpointsConfig.AddEndpoints(app);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();           
            app.UseSession();
            app.UseAuthentication();
            app.Services.GetRequiredService<EAMIAuthorizeAttribute>();
            //The call to UseCors must be placed after UseRouting, but before UseAuthorization
            // IIS: enable both Windows (GET/PUT/POST/DELETE/ETC) and Anonymous Authentication (used for OPTIONS/PreFlight)
            app.UseCors("AllowAll");  
            app.UseAuthorization();
            app.UseMiddleware<ProgramChoiceMiddleware>();
            //app.UseCors("AllowSpecificOrigin");
            app.MapControllers();
            app.Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .UseKestrel()
        //    .UseContentRoot(Directory.GetCurrentDirectory())
        //    .UseIISIntegration()
        //    .CaptureStartupErrors(true)
        //    .UseStartup<Startup>();

    }
}