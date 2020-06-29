

# Swagger



# Logging


First, install the Serilog.AspNetCore NuGet package into your app.

```json
dotnet add package Serilog.AspNetCore
```
or install by right click Solution file TodoApi.sln and select manage Nutget packages for solution.

In the search window paste `Serilog.AspNetCore`.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/InstallSerilog_Nuget.png?raw=true 'Request Pipeline')


Next, in your application's Program.cs file, configure Serilog first. A try/catch block will ensure any configuration issues are appropriately logged:


```C#
using Serilog;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
```


Then, add UseSerilog() to the Generic Host in CreateHostBuilder().


```C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <-- Add this line
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
}
```

[Serilog Docs](https://github.com/serilog/serilog-aspnetcore)

1. From the File menu, select New > Project.
2. Select the ASP.NET Core Web Application template and click Next.
3. Name the project TodoApi and click Create.
4. In the Create a new ASP.NET Core Web Application dialog, confirm that .NET Core and ASP.NET Core 3.1 are selected. Select the API template and click Create.

![alt text](https://github.com/Onemanwolf/.Net_Core_Api_Getting_Started/blob/master/Labs/images/CreateANewASPDotNetCoreWebApp.png?raw=true 'Request Pipeline')
