using Application.Helpers;
using Domain;
using Infrastructure;
using MediatR;

namespace Application.Queries;

public class GetAllQuery : IRequest<List<AmsterdamMakelaarsResponseModel>?>
{
    
}

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, List<AmsterdamMakelaarsResponseModel>?>
{
    private readonly IAmsterdamMakelaarsHttpClient _httpClient;

    public GetAllQueryHandler(IAmsterdamMakelaarsHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AmsterdamMakelaarsResponseModel>?> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        //Get all objects
        var queryParam = "";

        var responseList = await AmsterdamMakelaarsHelpers.GetAmsterdamMakelaarsResponseList(_httpClient, queryParam, cancellationToken);

        return responseList;
    }
}