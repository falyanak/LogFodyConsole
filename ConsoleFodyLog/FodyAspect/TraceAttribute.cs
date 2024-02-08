using ConsoleFodyLog;
using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static System.Console;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public class TraceAttribute : OnMethodBoundaryAspect
{
    private static readonly ILogger Logger = new LoggerFactory().AddLog4Net().CreateLogger<LogAttribute>();

    //  private static readonly Type AsyncTypeDef = typeof(Async<>);

    private static readonly Type AsyncTypeDef = typeof(AsyncStateMachineAttribute);
    private static readonly Type Tracer = typeof(TraceAttribute);
    private static readonly MethodInfo AsyncTracer = Tracer.GetMethod("TraceAsync");

    private void TraceEvent(MethodExecutionArgs args, long timestamp, string step, Task? task=null)
    {
        // Capture metrics here
        var msg = $"{step}  {args.Method.Name}";

        if (task != null)
        {
            var result = task.GetType().GetProperty("Result")?.GetValue(task);
            string jsonString = JsonSerializer.Serialize(result);

            WriteLine("Task : " + msg + " Retour = " + jsonString);
            Logger.LogInformation("Task {msg} Retour = {Paramètres} ", msg, jsonString);

            return;
        }

        if (args.Arguments.Length > 0)
        {
            string jsonString = JsonSerializer.Serialize(args.Arguments);

            WriteLine(msg + " Paramètres = " + jsonString);
            Logger.LogInformation("{msg} Paramètres = {Paramètres} ", msg, jsonString);
        }
        else
        {
            WriteLine(msg);
            Logger.LogInformation("{msg}", msg);
        }

      
    }

    private async Task<T> TraceAsync<T>(Task<T> asyncResult, Action trace)
    {
        var result = await asyncResult;
        trace();
        return result;
    }

    public override void OnEntry(MethodExecutionArgs args)
    {
        long timestamp = Stopwatch.GetTimestamp();
        TraceEvent(args, timestamp, "On entry");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        void Exit(Task? t) => TraceEvent(args, Stopwatch.GetTimestamp(), "On exit", t);

        if (args.ReturnValue is Task task)
        {
            task.ContinueWith(_ => Exit(task));
        }
        else
        {
            Type clrType = args.ReturnValue.GetType();
            if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == AsyncTypeDef)
            {
                Type[] generics = clrType.GetGenericArguments();
                var result = AsyncTracer.MakeGenericMethod(generics)
                    .Invoke(this, new object[] { args.ReturnValue});

                args.ReturnValue = result;
            }
            else
            {
                Exit(null);
            }
        }
    }
}