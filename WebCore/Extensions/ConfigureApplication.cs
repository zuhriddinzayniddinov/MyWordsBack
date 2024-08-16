using System.Text;
using System.Text.Json.Serialization;
using AuthenticationBroker.Options;
using DatabaseBroker.DataContext;
using Entity.Enum;
using Entity.Extensions;
using Entity.Models;
using Entity.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NCrontab;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using WebCore.CronSchedulers;
using WebCore.Filters;
using WebCore.Middlewares;
using WebCore.Models;

namespace WebCore.Extensions;

public static class ConfigureApplication
{
    public static void ConfigureDefault(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<TelegramBotCredential>(builder.Configuration
            .GetSection("TelegramBotCredential"));
        
        builder.WebHost.UseUrls(builder.Configuration.GetConnectionString("Host")!);
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .WriteTo
            .Api(builder.Services.BuildServiceProvider().GetService<IOptions<TelegramBotCredential>>(),LogEventLevel.Fatal)
            .WriteTo
            .File("logs/log-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        builder
            .Configuration
            .AddEnvironmentVariables()
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

        builder.Services.Configure<JwtOption>(builder.Configuration
            .GetSection("JwtOption"));

        builder
            .Services
            .AddDbContextPool<ProgramDataContext>(optionsBuilder =>
            {
                optionsBuilder
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString"));
                optionsBuilder.UseLoggerFactory(new SerilogLoggerFactory(Log.Logger));
                optionsBuilder
                    .UseLazyLoadingProxies();
            });

        builder
            .Services
            .AddHealthChecks();

        builder
            .Services
            .AddCors(options =>
            {
                options
                    .AddDefaultPolicy(builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

        builder.Services
            .Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options
                .AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

            options
                .AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
        });

        builder.Services
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptionsSection = builder.Configuration.GetSection("JwtOption");

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptionsSection["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtOptionsSection["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptionsSection["SecretKey"])),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


        builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();
    }

    public static async Task ConfigureDefault(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var dataContext = scope.ServiceProvider.GetService<ProgramDataContext>();
        Log.Information("{0}", "Migrations applying...");
        await dataContext?.Database.MigrateAsync()!;
        Log.Information("{0}", "Migrations applied.");
        scope.Dispose();


        app.UseCors();
        app.UseHealthChecks("/healths");

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        await app.SynchronizePermissionsAsync();
        await app.InitSidDataAsync();
    }

    private static async Task SynchronizePermissionsAsync(this WebApplication app)
    {
        try
        {
            Log.Information("Permissions synchronization starting....");
            using var scope = app.Services.CreateScope();
            await using var dataContext = scope.ServiceProvider.GetService<ProgramDataContext>();

            ArgumentNullException.ThrowIfNull(dataContext);

            var permissionCodes = typeof(UserPermissions).GetEnumValues().Cast<object>();

            var enumerable = permissionCodes as object[] ?? permissionCodes.ToArray();
            foreach (var permissionCode in enumerable)
            {
                var storedCode = await dataContext.Permissions.FirstOrDefaultAsync(x => x.Code == (int)permissionCode);
                if (storedCode is not null) continue;
                await dataContext.Permissions.AddAsync(new Permission()
                {
                    Code = (int)permissionCode,
                    Name = permissionCode.ToString() ?? string.Empty
                });
                continue;
            }

            await dataContext.SaveChangesAsync();

            var codes = enumerable.Cast<int>();

            var removedCodes = await dataContext
                .Permissions
                .Where(x => codes.All(pc => pc != x.Code))
                .ToListAsync();

            if (!removedCodes.IsNullOrEmpty())
            {
                dataContext.Permissions.RemoveRange(removedCodes);
                await dataContext.SaveChangesAsync();
            }

            Log.Information("Permissions synchronization finished successfully.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Permissions synchronization crashed.");
        }
    }
    private static async Task InitSidDataAsync(this WebApplication app)
    {
        try
        {
            Log.Information("Init sid data starting....");
            using var scope = app.Services.CreateScope();
            await using var dataContext = scope.ServiceProvider.GetService<ProgramDataContext>();

            ArgumentNullException.ThrowIfNull(dataContext);

            var permissionCodes = typeof(UserPermissions).GetEnumValues().Cast<object>();

            var enumerable = permissionCodes as object[] ?? permissionCodes.ToArray();
            var structureDefault = await dataContext.Structures
                .Where(s => !s.IsDelete)
                .FirstOrDefaultAsync(s => s.IsDefault);
            
            if (structureDefault is null)
            {
                var structureEntity = await dataContext.Structures.AddAsync(new Structure()
                {
                    IsDefault = true,
                    Name = "Default"
                });

                await dataContext.SaveChangesAsync();

                structureDefault = structureEntity.Entity;
            }
            var structureSuperAdmin = await dataContext.Structures
                .Where(s => !s.IsDelete)
                .FirstOrDefaultAsync(s => s.Id == -1);
            
            if (structureSuperAdmin is null)
            {
                var structureSuperAdminEntity = await dataContext.Structures.AddAsync(new Structure()
                {
                    Id = -1,
                    IsDefault = false,
                    Name = "Super Admin"
                });

                await dataContext.SaveChangesAsync();

                structureSuperAdmin = structureSuperAdminEntity.Entity;
            }
            foreach (var permissionCode in enumerable)
            {
                var storedCode = await dataContext.Permissions
                    .Where(p => !p.IsDelete)
                    .FirstOrDefaultAsync(x => x.Code == (int)permissionCode);
                if(storedCode is null) continue;
                
                if ((permissionCode.ToString() ?? string.Empty).StartsWith("Default"))
                {
                    var structurePermissionDefault = await dataContext.StructurePermissions
                        .Where(sp => !sp.IsDelete)
                        .Where(sp => sp.PermissionId == storedCode.Id)
                        .FirstOrDefaultAsync(ap => ap.StructureId == structureDefault.Id);
                    
                    if(structurePermissionDefault is null)
                        await dataContext.StructurePermissions.AddAsync(new StructurePermission()
                        {
                            StructureId = structureDefault.Id,
                            Structure = structureDefault,
                            Permission = storedCode,
                            PermissionId = storedCode.Id,
                        });
                }
                
                var structurePermissionAdmin = await dataContext.StructurePermissions
                    .Where(sp => !sp.IsDelete)
                    .Where(sp => sp.PermissionId == storedCode.Id)
                    .FirstOrDefaultAsync(sp => sp.StructureId == structureSuperAdmin.Id);
                
                if(structurePermissionAdmin is null)
                    await dataContext.StructurePermissions.AddAsync(new StructurePermission()
                    {
                        StructureId = structureSuperAdmin.Id,
                        Structure = structureSuperAdmin,
                        Permission = storedCode,
                        PermissionId = storedCode.Id,
                    });
            }

            var admin = await dataContext.Users
                .Where(u => !u.IsDelete)
                .FirstOrDefaultAsync(u => u.Id == -1);
            
            if(admin is null)
                await dataContext.Users.AddAsync(new User()
                {
                    Id = -1,
                    FirstName = "Zuhriddin",
                    LastName = "Zayniddinov",
                    Structure = structureSuperAdmin,
                    SignMethods = new []
                    {
                        new PasswordSignMethod()
                        {
                            Type = SignMethods.Password,
                            PhoneNumber = "+998936675925",
                            PasswordHash = PasswordHelper.Encrypt("12345678")
                        }
                    },
                });

            await dataContext.SaveChangesAsync();

            Log.Information("Init sid data finished successfully.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Init sid data crashed.");
        }
    }
    public static IServiceCollection AddCronJob<T>(this IServiceCollection services, string cronExpression)
        where T : class, ICronJob
    {
        var cron = CrontabSchedule.TryParse(cronExpression)
                   ?? throw new ArgumentException("Invalid cron expression", nameof(cronExpression));

        var entry = new CronRegistryEntry(typeof(T), cron);

        services.AddHostedService<CronScheduler>();
        services.TryAddSingleton<T>();
        services.AddSingleton(entry);

        return services;
    }
}