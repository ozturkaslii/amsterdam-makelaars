using System.Net.Http.Json;
using Castle.Core.Configuration;
using Domain;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class AmsterdamMakelaarsHttpClient : IAmsterdamMakelaarsHttpClient
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IOptionsMonitor<HttpClients> _options;

    public AmsterdamMakelaarsHttpClient(IHttpClientFactory clientFactory,
        IOptionsMonitor<HttpClients> options)
    {
        _clientFactory = clientFactory;
        _options = options;
    }
    
    private const string Client = "funda";

    public async Task<AmsterdamMakelaarsModel?> GetAsync(string queryParam, int currentPage, CancellationToken cancellationToken)
    {
        var httpClient = _clientFactory.CreateClient(Client);
        var temporaryKey = _options.CurrentValue.AmsterdamMakelaarHttpClient.TemporaryKey;
        
        var makelaarsModel = await httpClient.GetFromJsonAsync<AmsterdamMakelaarsModel>
        ($"feeds/Aanbod.svc/json/{temporaryKey}/?type=koop&zo=/amsterdam{queryParam}/&page={currentPage}&p%20agesize=25",
            cancellationToken);

        if (makelaarsModel != null && !makelaarsModel.Objects.Any())
        {
            return null;
        }

        return makelaarsModel;
    }
}

public interface IAmsterdamMakelaarsHttpClient
{
    Task<AmsterdamMakelaarsModel?> GetAsync(string queryParam, int currentPage, CancellationToken cancellationToken);
}
