using Microsoft.Extensions.Options;
using wyt.Services;

namespace wyt;

public class App
{
    private readonly ILogger<App> _logger;
    private readonly AppSettings _settings;
    private readonly NetworkSettingsPinger _network;
    private readonly IPingerService _pinger;
    private readonly ICrawlerService _crawler;
    private const int TIMEOUT = 5;

    public static CancellationToken TokenTimeout
    {
        get
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            cts.CancelAfter(TimeSpan.FromSeconds(TIMEOUT));
            return token;
        }
    }

    public static CancellationToken TokenCancel
    {
        get
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            cts.Cancel();
            return token;
        }
    }

    public App(ILogger<App> logger,
        IOptions<AppSettings> settings,
        IPingerService pinger,
        ICrawlerService crawler,
        IOptions<NetworkSettingsPinger> network)
    {
        _logger = logger;
        _settings = settings.Value;
        _pinger = pinger;
        _crawler = crawler;
        _network = network.Value;
    }
    public async Task Ping(string host)
    {
        _logger.LogInformation("Ping scan!");
        await _pinger.Ping(host, CancellationToken.None);        
    }
}