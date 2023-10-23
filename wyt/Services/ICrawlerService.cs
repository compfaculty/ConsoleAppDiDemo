namespace wyt.Services;

public interface ICrawlerService
{
    Task<string> GetUrl(string url, CancellationToken token);
    Task<IEnumerable<string?>> ParseBaseUrlsFromConfig(string baseUrl, CancellationToken token);
}