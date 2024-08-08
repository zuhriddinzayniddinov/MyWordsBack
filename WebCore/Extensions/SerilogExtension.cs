using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Core;
using WebCore.Models;

public static class SerilogExtension
{
    public static LoggerConfiguration Api(
        this LoggerSinkConfiguration loggerConfiguration,
        IOptions<TelegramBotCredential>? botCredential,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
    {
        if (loggerConfiguration == null)
            throw new ArgumentNullException(nameof(loggerConfiguration));

        return loggerConfiguration.Sink(
            new ApiSink(botCredential),
            restrictedToMinimumLevel);
    }
}

public class ApiSink : ILogEventSink
{
    private readonly TelegramBotCredential _botCredential;
    public ApiSink(IOptions<TelegramBotCredential>? botCredential)
    {
        _botCredential = botCredential.Value;
    }
    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage();

        if (logEvent.Exception != null)
        {
            message += $"\n\nException: {logEvent.Exception?.GetType().FullName};";
            message += $"\n\nMessage: {logEvent?.Exception?.Message};";
            try
            {
                var arr = logEvent.Exception?.StackTrace?.Split("  at ")
                    .Where(t =>
                        !t.StartsWith("Microsoft") &&
                        !t.StartsWith("System")).ToArray();
                
                message += $"\n\n```StackTrace\n{arr?[1]}\n```";
                message += $"\n\n```StackTrace\n{arr?[2]}\n```";
            }
            catch (Exception e)
            {
                Log.Error(e.Message,e);
            }
        }

        var url = $"{_botCredential.Domain}/bot{_botCredential.Token}/sendMessage?" +
                  $"chat_id={_botCredential.ChatId}&text={message}&parse_mode=Markdown";

        using var client = new HttpClient();
        var response = client.GetAsync(url).Result;
    }
}