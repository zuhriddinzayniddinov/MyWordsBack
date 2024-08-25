using AuthenticationBroker.TokenHandler;
using AuthService.Services;
using DatabaseBroker.Repositories.Auth;
using DatabaseBroker.Repositories.StaticFiles;
using DatabaseBroker.Repositories.Words;
using RoleService.Service;
using StaticFileService.Service;
using WordsService.GroupService;
using WordsService.LanguageService;
using WordsService.WordService;

namespace TemplateApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<IStaticFileService, StaticFileService.Service.StaticFileService>();
        services.AddScoped<IAuthService, AuthService.Services.AuthService>();
        services.AddScoped<IRoleService, RoleService.Service.RoleService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IWordService, WordService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStructureRepository, StructureRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IStructurePermissionRepository, StructurePermissionRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
        services.AddScoped<ISignMethodsRepository, SignMethodsRepository>();
        services.AddScoped<IStaticFileRepository, StaticFileRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IWordRepository, WordRepository>();

        return services;
    }
}