namespace wyt;

public static class Extentions
{
    public static string UrlToHostname(this string url)
    {
        Uri uri = new(url);
        return uri.Host.Trim();
    }
}