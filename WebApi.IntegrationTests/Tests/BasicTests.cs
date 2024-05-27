using System.Net;
using System.Text.Json;
using DataModel.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using WebApi.IntegrationTests.Helpers;



public class BasicTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
 
    public BasicTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
    }
   
    [Theory]
    [InlineData("/api/colaborator")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
    [Fact]
    public async Task GetColaborators_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AbsanteeContext>();
 
            Utilities.ReinitializeDbForTests(db);
            Utilities.InitializeDbForTests(db);
        }
 
        var response = await _client.GetAsync("/api/colaborator");
 
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(responseBody), "Response body is null or empty");
 
 
        var jsonDocument = JsonDocument.Parse(responseBody);
        var jsonArray = jsonDocument.RootElement;
 
        Assert.True(jsonArray.ValueKind == JsonValueKind.Array, "Response body is not a JSON array");
        Assert.Equal(3, jsonArray.GetArrayLength());
    }

    [Fact]
    public async Task GetColaborators_ReturnsEmptyArrayWhenNoData()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AbsanteeContext>();

            Utilities.ReinitializeDbForTests(db); // Ensure the database is empty
        }

        // Act
        var response = await _client.GetAsync("/api/colaborator");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseBody = await response.Content.ReadAsStringAsync();
        var colaborators = JsonConvert.DeserializeObject<List<Application.DTO.ColaboratorDTO>>(responseBody);

        Assert.Empty(colaborators);
    }
    [Fact]
    public async Task GetColaborators_ReturnsSeedData()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AbsanteeContext>();

            Utilities.InitializeDbForTests(db); // Ensure the database is initialized with seed data
        }

        // Act
        var response = await _client.GetAsync("/api/colaborator");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseBody = await response.Content.ReadAsStringAsync();
        var colaborators = JsonConvert.DeserializeObject<List<Application.DTO.ColaboratorDTO>>(responseBody);

        Assert.Equal(3, colaborators.Count);
        Assert.Equal("Catarina Moreira", colaborators[0].Name);
        Assert.Equal("a", colaborators[1].Name);
        Assert.Equal("kasdjflkadjf lkasdfj laksdjf alkdsfjv alkdsfjv asl", colaborators[2].Name);
    }

    [Theory]
    [InlineData("/api/invalidendpoint")]
    public async Task Get_InvalidEndpoint_ReturnsNotFound(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetColaborators_ResponseTimeIsAcceptable()
    {
        // Arrange
        var maxResponseTime = TimeSpan.FromSeconds(2); // Define maximum acceptable response time

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/colaborator");
        stopwatch.Stop();

        // assert
        response.EnsureSuccessStatusCode();
        Assert.True(stopwatch.Elapsed < maxResponseTime, $"Response time exceeded {maxResponseTime.TotalSeconds} seconds");
    }

 

}
    






