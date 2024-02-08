using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static System.Console;

namespace ConsoleFodyLog;

public sealed class LogAttribute : OnMethodBoundaryAspect
{
    private static readonly ILogger Logger = new LoggerFactory().AddLog4Net().CreateLogger<LogAttribute>();

    public override void OnEntry(MethodExecutionArgs args)
    {
        var msg = "On entry " + args.Method.Name;

        if (args.Arguments.Length > 0)
        {
            string jsonString = JsonSerializer.Serialize(args.Arguments);

            WriteLine(msg + " Paramètres = " + jsonString);

            Logger.LogInformation("{msg} Paramètres = {Paramètres} ", msg, jsonString);
        }
        else
        {
            WriteLine(msg);
            Logger.LogInformation(msg);
        }

    }
    public override void OnExit(MethodExecutionArgs args)
    {
        var msg = $"LogAttribute On exit {args.Method.Name} retour = ";

        if (args.ReturnValue is Task task)
        {
            if (!task.IsFaulted)
            {
                task.ContinueWith(task =>
                {
                    var result = task.GetType().GetProperty("Result")?.GetValue(task);

                    string jsonString = JsonSerializer.Serialize(result);
                    WriteLine("LogAttribute Task : " + msg + jsonString);
                    Logger.LogInformation("LogAttribute Task : {msg} {Paramètres} ", msg, jsonString);
                });
            }
        }
        else
        if (args.ReturnValue != null)
        {
            args.ReturnValue = (args.ReturnValue as string);
            string jsonString = JsonSerializer.Serialize(args.ReturnValue);

            WriteLine(msg + jsonString);
            Logger.LogInformation(msg + jsonString);
        }
    }

    public override void OnException(MethodExecutionArgs args)
    {
        WriteLine("UNE EXCEPTION s'est produite avec le message : " + args.Exception.Message);
        Logger.LogError("On exception s'est produite avec le message : " + args.Exception.Message);
    }
}