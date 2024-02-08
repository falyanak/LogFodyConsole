using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static System.Console;

namespace ConsoleFodyLog;

public sealed class HandleExceptionAttribute : OnMethodBoundaryAspect
{
    private static readonly ILogger Logger = new LoggerFactory().AddLog4Net().CreateLogger<HandleExceptionAttribute>();

    public override void OnExit(MethodExecutionArgs args)
    {
        var msg = $"HandleExceptionAttribute On exit {args.Method.Name} retour = ";

        if (args.ReturnValue is Task { IsFaulted: false } task)
        {
            task.ContinueWith(t =>
            {
                var result = task.GetType().GetProperty("Result")?.GetValue(task);

                string jsonString = JsonSerializer.Serialize(result);
                WriteLine("HandleExceptionAttribute Task : " + msg + jsonString);
                Logger.LogInformation("HandleExceptionAttribute Task : {msg} {Paramètres} ", msg, jsonString);
            });
        }
    }
}