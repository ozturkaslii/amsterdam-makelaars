using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Helpers;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using WebAPI.Controllers;
using Xunit;
using Object = Domain.Object;

namespace AmsterdamMakelaarsTest;

public class AmsterdamMakelaarsUnitTests
{
    private readonly RealEstatesController _controller;

    public AmsterdamMakelaarsUnitTests()
    {
        var logger = new Mock<ILogger>();
        var mediator = new Mock<IMediator>();
        _controller = new RealEstatesController(logger.Object, mediator.Object);
    }

    [Fact]
    public void GetOrderedDictionary_ShouldReturn_ExpectedResult()
    {
        var model = new AmsterdamMakelaarsModel
        {
            Paging = new Paging
            {
                AantalPaginas = 2,
                HuidigePagina = 1
            },
            Objects = new List<Object>
            {
                new()
                {
                    MakelaarId = 1,
                    MakelaarNaam = "Elan Makelaars",
                    Woonplaats = "Amsterdam",
                    VerkoopStatus = ""
                },
                new()
                {
                    MakelaarId = 2,
                    MakelaarNaam = "Eefje Voogd Makelaardij",
                    Woonplaats = "Amsterdam",
                    VerkoopStatus = ""
                },
                new()
                {
                    MakelaarId = 1,
                    MakelaarNaam = "Elan Makelaars",
                    Woonplaats = "Amsterdam",
                    VerkoopStatus = ""
                }
            },
            TotaalAantalObjecten = 3
        };

        var expected = new Dictionary<string, int>
        {
            { "Elan Makelaars", 2 },
            { "Eefje Voogd Makelaardij", 1 }
        };
        var asd = AmsterdamMakelaarsHelpers.GetOrderedDictionary(model);
        Assert.Equal(expected, asd);
    }

    [Fact]
    public void ReturnOrderedDictionary_ShouldReturn_ExpectedResult()
    {
        var currentDict = new Dictionary<string, int>()
        {
            { "Elan Makelaars", 2 },
            { "Eefje Voogd Makelaardij", 1 }
        };

        var newDict = new Dictionary<string, int>()
        {
            { "Hoen Makelaars", 2 },
            { "Keizerskroon Makelaars", 1 },
            { "Eefje Voogd Makelaardij", 2 }
        };

        var expectedResult = new Dictionary<string, int>()
        {
            { "Elan Makelaars", 2 },
            { "Eefje Voogd Makelaardij", 3 },
            { "Hoen Makelaars", 2 },
            { "Keizerskroon Makelaars", 1 }
        };

        var actual = AmsterdamMakelaarsHelpers.ReturnOrderedDictionary(currentDict, newDict);
        Assert.Equal(expectedResult, actual);
    }

    [Fact]
    public async Task GetTuinList_ShouldReturn_Status200OK_Or_Status400BadRequest()
    {
        var actionResult = await _controller.GetTuinList(CancellationToken.None);
        var statusCode = ((OkObjectResult)actionResult).StatusCode;
        switch (statusCode)
        {
            case 200:
            {
                Assert.Equal(StatusCodes.Status200OK, statusCode.Value);
                break;
            }
            case 400:
                Assert.Equal(StatusCodes.Status400BadRequest, statusCode.Value);
                break;
        }
    }
    
    [Fact]
    public async Task GetAllList_ShouldReturn_Status200OK_Or_Status400BadRequest()
    {
        var actionResult = await _controller.GetAllList(CancellationToken.None);
        var statusCode = ((OkObjectResult)actionResult).StatusCode;
        switch (statusCode)
        {
            case 200:
            {
                Assert.Equal(StatusCodes.Status200OK, statusCode.Value);
                break;
            }
            case 400:
                Assert.Equal(StatusCodes.Status400BadRequest, statusCode.Value);
                break;
        }
    }
}