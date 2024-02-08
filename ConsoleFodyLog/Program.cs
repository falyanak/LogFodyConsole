using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ConsoleFodyLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Console;

//[MemoryDiagnoser]
public class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<Sample>();

    })
    .ConfigureLogging(logBuilder =>
    {
        // injection du logger
        logBuilder.SetMinimumLevel(LogLevel.Trace);
        logBuilder.AddLog4Net("log4net.config");

    }).UseConsoleLifetime();

        var host = builder.Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            try
            {
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Dans Program Main"); // test OK

                // injection du logger
                var sample = new Sample(logger);

                var taskResult = await sample.ManagePerson();

                if (taskResult.IsCompleted)
                {
                    WriteLine("Tâche complète = " + taskResult.IsCompleted);
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error occured {ex.Message}");
                return 1;
            }
        }

        // test Benchmark
        //var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
        return 0;
    }
}
