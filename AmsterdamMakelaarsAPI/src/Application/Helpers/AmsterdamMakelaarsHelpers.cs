using Domain;
using Infrastructure;
using Serilog;

namespace Application.Helpers;

public static class AmsterdamMakelaarsHelpers
{
    public static async Task<List<AmsterdamMakelaarsResponseModel>> GetAmsterdamMakelaarsResponseList(
        IAmsterdamMakelaarsHttpClient httpClient, string queryParam, CancellationToken cancellationToken)
    {
        //Get the first page's result
        var currentPage = 1;
        var realEstateModel = await httpClient.GetAsync(queryParam,currentPage, cancellationToken);

        //If we don't have any object, we return null
        if (realEstateModel == null || !realEstateModel.Objects.Any())
        {
            Log.Logger.Warning("Real estate model is empty");
            return await Task.FromResult<List<AmsterdamMakelaarsResponseModel>>(null);
        }

        //Total object count is coming from TotaalAantalObjecten property
        var totalObjects = realEstateModel.TotaalAantalObjecten;
        var pageSize = 15;
        
        //Group same listings that come from same real estate agents and order them
        var resultDict = GetOrderedDictionary(realEstateModel);
        while (totalObjects >= currentPage * pageSize)
        {
            //increment the current page
            currentPage++;

            //Get incremented page's result
            realEstateModel = await httpClient.GetAsync(queryParam,currentPage, cancellationToken);
            
            //If we dont have any object in that page, continue
            if(realEstateModel == null && !realEstateModel.Objects.Any())
                continue;
            
            //Get ordered dictionary of the current page
            var newOrderedDictionary = GetOrderedDictionary(realEstateModel);

            //Combine grouped and ordered dictionaries
            //put them into resultDict
            resultDict = ReturnOrderedDictionary(resultDict, newOrderedDictionary);
        }

        //Get top 10 of result dict
        var topTenResult = resultDict.Take(10);

        //Create the response model of top 10 result
        var responseList = new List<AmsterdamMakelaarsResponseModel>();
        foreach (var (key, value) in topTenResult)
        {
            var model = new AmsterdamMakelaarsResponseModel
            {
                RealEstateAgent = key,
                ListingCount = value
            };

            responseList.Add(model);
        }
        
        return responseList;
    }
    
    /// <summary>
    /// Group objects by MakelaarNaam and Woonplaats
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static Dictionary<string, int>? GetOrderedDictionary(AmsterdamMakelaarsModel model)
    {
        //It is better to group by with MakelaarNaam because MakelaarId may be different
        //Von Poll Real Estate MakelaarId is 24789 on p.31 and 24820 on p.33

        return model.Objects
            .GroupBy(q => q.MakelaarNaam,
                q => q.Woonplaats,
                (key, g) => new AmsterdamMakelaarsResponseModel{ RealEstateAgent = key, ListingCount = g.Count() })
            .OrderByDescending(q => q.ListingCount)
            .ToDictionary(q=> q.RealEstateAgent, v => v.ListingCount);
    }

    /// <summary>
    /// This method compares two dictionaries and returns a new one. the comparison is based on dictionary key.
    /// It returns a new, and ordered dictionary.
    /// </summary>
    /// <param name="currentDict"></param>
    /// <param name="newDict"></param>
    /// <returns></returns>
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