using AuthService.Services;
using WebCore.CronSchedulers;

namespace AuthApi.CronJobs;

public abstract class TemplateCroneJob(
    ILogger<TemplateCroneJob> logger,
    IServiceProvider serviceProvider)
    : ICronJob
{
    public Task Run(CancellationToken token = default)
    {
        logger.Log(LogLevel.Information,"Start template cron");
        using var scope = serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        /*authService.Register(null);*/
        logger.Log(LogLevel.Information,"Finish template cron");
        return Task.CompletedTask;
    }
}