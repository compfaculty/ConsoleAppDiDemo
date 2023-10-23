using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;

namespace wyt.Services;

public class Crawler : ICrawlerService
{
    private readonly ILogger<Crawler> _logger;
    //private readonly NetworkSettingsPinger _config;
    private readonly HttpClient _browser = new();

    public Crawler(ILogger<Crawler> logger, IOptions<NetworkSettingsPinger> config)
    {
        _logger = logger;
        //_config = config.Value;
    }

    public async Task<string> GetUrl(string url, CancellationToken token)
    {
        string ret;
        try
        {
            var respond = await _browser.GetAsync(url, token).ConfigureAwait(false);
            ret = $"Status {url}: {respond?.StatusCode}";
        }
        catch (Exception e)
        {
            _logger.LogWarning("Looks {Url} is dead...{EMessage}", url, e.Message);
            ret = $"The status {url}: no response";
        }

        return ret;
    }

    public async Task<IEnumerable<string?>> ParseBaseUrlsFromConfig(string baseUrl, CancellationToken token)
    {
        _logger.LogInformation("Start parsing base url {BaseUrl}", baseUrl);
        //using  AngleSharp
        var config = Configuration.Default.WithDefaultLoader();
        var document = await BrowsingContext.New(config).OpenAsync(new Url(baseUrl), token);

        var urlsFound = document.All.Where(m => m.LocalName == "a" && m.HasAttribute("href"))
            .Select(elem => elem.GetAttribute("href"))
            .Where(href => href is not null && href.StartsWith("http"))
            .ToList()
            .Distinct();
        return urlsFound;
       
    }
}