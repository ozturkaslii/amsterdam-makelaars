namespace Infrastructure.Configurations;

public class HttpClients
{
    public AmsterdamMakelaarHttpClient AmsterdamMakelaarHttpClient { get; set; }
}

public class AmsterdamMakelaarHttpClient
{
    public string BaseUri { get; set; }
    public string TemporaryKey { get; set; }
}