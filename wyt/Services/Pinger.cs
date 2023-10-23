using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;

namespace wyt.Services;

public class Pinger : IPingerService
{
    private readonly ILogger<Pinger> _logger;
    private readonly NetworkSettingsPinger _config;
    private readonly Random rnd;

    public Pinger(ILogger<Pinger> logger, IOptions<NetworkSettingsPinger> config)
    {
        _logger = logger;
        _config = config.Value;
        rnd = new Random();
    }

    public async Task Ping(string host, CancellationToken token, int timeout = 0, int bufferSize = 32,
        bool fragment = true, int ttl = 128)
    {
        Ping pinger = new();
        PingOptions options = new()
        {
            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            DontFragment = fragment,
            Ttl = ttl
        };

        // Create a buffer of 32 bytes of data to be transmitted.
        var buffer = new Byte[bufferSize];
        rnd.NextBytes(buffer);
        try
        {
            var reply = await pinger.SendPingAsync(host, timeout, buffer, options);


            if (reply.Status == IPStatus.Success)

            {
                var info = $"Host: {host} : {reply.Status}, {reply.Address}\nRoundTrip time: {reply.RoundtripTime} "
                       + $" Time to live: {reply?.Options?.Ttl}, Don't fragment: {reply?.Options?.DontFragment} "
                       + $" Buffer size: {reply?.Buffer.Length}";


                _logger.LogInformation(info);
            }
        }
        catch (Exception ex)
        {
            string message = $"Failed  for host {host}, {ex.Message}";
            _logger.LogError(message);
        }
    }
}