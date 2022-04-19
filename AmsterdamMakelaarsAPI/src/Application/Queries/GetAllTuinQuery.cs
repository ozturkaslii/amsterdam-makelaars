using Application.Helpers;
using Domain;
using Infrastructure;
using MediatR;

namespace Application.Queries;

public class GetAllTuinQuery : IRequest<List<AmsterdamMakelaarsResponseModel>?>
{
    
}

public class GetAllTuinQueryHandler : IRequestHandler<GetAllTuinQuery, List<AmsterdamMakelaarsResponseModel>?>
{
    private readonly IAmsterdamMakelaarsHttpClient _httpClient;

    public GetAllTuinQueryHandler(IAmsterdamMakelaarsHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AmsterdamMakelaarsResponseModel>?> Handle(GetAllTuinQuery request, CancellationToken cancellationToken)
    {
        //Get all tuins
        var queryParam = "/tuin";
        
        var responseList = await AmsterdamMakelaarsHelpers.GetAmsterdamMakelaarsResponseList(_httpClient, 
            queryParam, cancellationToken);

        return responseList;
    }
}