using Domain;
using Infrastructure;
using Serilog;

namespace Application.Helpers;

public static class AmsterdamMakelaarsHelpers
{
    public static async Task<List<AmsterdamMakelaarsResponseModel>> GetAmsterdamMakelaarsResponseList(
        IAmsterdamMakelaarsHttpClient httpClient, string queryParam, CancellationToken cancellationToken)
    {
        //TODO: refactor naming
        var currentPage = 1;
        var realEstateModel = await httpClient.GetAsync(queryParam,currentPage, cancellationToken);

        if (realEstateModel == null || !realEstateModel.Objects.Any())
        {
            Log.Logger.Warning("Real estate model is empty");
            return await Task.FromResult<List<AmsterdamMakelaarsResponseModel>>(null);
        }

        var totalObjects = realEstateModel.TotaalAantalObjecten;
        var pageSize = 15;
        
        var orderedList = GetOrderedDictionary(realEstateModel);
        var resultDict = new Dictionary<string, int>();
        while (totalObjects >= currentPage * pageSize)
        {
            currentPage++;

            realEstateModel = await httpClient.GetAsync(queryParam,currentPage, cancellationToken);
            if(realEstateModel == null && !realEstateModel.Objects.Any())
                continue;
            
            var newOrderedList = GetOrderedDictionary(realEstateModel);

            resultDict = ReturnOrderedDictionary(orderedList, newOrderedList);
        }

        var topTenResult = resultDict.Take(10);

        var responseList = new List<AmsterdamMakelaarsResponseModel>();
        foreach (var (key, value) in topTenResult)
        {
            var model = new AmsterdamMakelaarsResponseModel
            {
                MakelaarNaam = key,
                Places = value
            };

            responseList.Add(model);
        }
        
        return responseList;
    }
    
    public static Dictionary<string, int>? GetOrderedDictionary(AmsterdamMakelaarsModel model)
    {
        //It is better to group by with MakelaarNaam because MakelaarId may be different
        //Von Poll Real Estate MakelaarId is 24789 on p.31 and 24820 on p.33

        return model.Objects
            .GroupBy(q => q.MakelaarNaam,
                q => q.Woonplaats,
                (key, g) => new AmsterdamMakelaarsResponseModel{ MakelaarNaam = key, Places = g.Count() })
            .OrderByDescending(q => q.Places)
            .ToDictionary(q=> q.MakelaarNaam, v => v.Places);
    }

    public static Dictionary<string, int> ReturnOrderedDictionary(Dictionary<string, int> currentDict, Dictionary<string, int> newDict)
    {
        foreach (KeyValuePair<string,int> keyValuePair in newDict)
        {
            if (currentDict.ContainsKey(keyValuePair.Key))
            {
                currentDict[keyValuePair.Key] += newDict[keyValuePair.Key];
            }
            else
            {
                currentDict.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        return currentDict.OrderByDescending(q => q.Value)
            .ToDictionary(k => k.Key, v => v.Value);
    }
}