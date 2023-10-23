using wyt;
using Serilog;
using wyt.Services;

//working with configuration
IConfigurationRoot config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("app-settings.json", false, true)
    .Build();

//setup main Serilog logger using config 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(logbuilder => logbuilder.AddSerilog(dispose: true));
        services.AddOptions();
        services.Configure<AppSettings>(config.GetSection("AppSettings"));
        services.Configure<NetworkSettingsPinger>(config.GetSection("PingerSettings"));
        services.AddSingleton<App>();
        services.AddTransient<ICrawlerService, Crawler>();
        services.AddTransient<IPingerService, Pinger>();
        services.AddHostedService<Worker>();
        
    })
    .UseSerilog()
    .Build();

/* example Crawler
var tCrawler = host.Services.GetService<ICrawlerService>();
if (tCrawler is not null)
{
    var res = await tCrawler.GetUrl("https://www.youtube.com/@zzzz", CancellationToken.None);
    Log.Logger.Information(res);
}
*/

var app = host.Services.GetService<App>();
if (app is not null)
{
    Log.Information("Ping service has started...");
    await app.Ping("8.8.8.8")!;
    Log.Logger.Warning("Ping service : Done!");

}

await host.RunAsync();