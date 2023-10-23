namespace wyt.Services;

public interface IPingerService
{
    Task Ping(string host, 
        CancellationToken token,
        int timeout = 120,
        int bufferSize = 32,
        bool fragment = true,
        int ttl = 100);
}