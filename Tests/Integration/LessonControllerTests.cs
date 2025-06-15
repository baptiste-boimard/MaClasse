using System;
using System.Net;
using System.Net.Http.Json;

using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Service.Database.Controllers;
using Service.Database.Interfaces;
using Service.Database.Services;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Tests.Integration;

public class LessonControllerTests : IClassFixture<WebApplicationFactory<Service.Database.Program>>
{
    private readonly WebApplicationFactory<Service.Database.Program> _factory;
    private readonly Mock<ILessonRepository> _repoMock = new();
    private class ConstantHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public ConstantHandler(HttpResponseMessage response) { _response = response; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_response.StatusCode){Content = _response.Content});
        }
    }

    public LessonControllerTests(WebApplicationFactory<Service.Database.Program> factory)
    {
        var userHandler = new ConstantHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new SessionData { UserId = "1" })
        });
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Url:ApiGateway","http://localhost"}}).Build();
        var userService = new UserService(new HttpClient(userHandler){BaseAddress = new Uri("http://localhost")}, config);

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ILessonRepository));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton(_repoMock.Object);
                var userDesc = services.SingleOrDefault(d => d.ServiceType == typeof(UserService));
                if (userDesc != null) services.Remove(userDesc);
                services.AddSingleton(userService);
            });
        });
    }

    [Fact]
    public async Task GetDocument_ReturnsNotFound_WhenMissing()
    {
        _repoMock.Setup(r => r.GetDocument(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Document?)null);
        var client = _factory.CreateClient();
        var request = new FileRequestToDatabase { IdUser = "1", Document = new Document { IdDocument = "42" } };
        var response = await client.PostAsJsonAsync("/api/get-document", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_ReturnsDocument_WhenExists()
    {
        var doc = new Document { IdDocument = "42", Name = "Test" };
        _repoMock.Setup(r => r.GetDocument("42", "1")).ReturnsAsync(doc);
        var client = _factory.CreateClient();
        var request = new FileRequestToDatabase { IdUser = "1", Document = new Document { IdDocument = "42" } };
        var response = await client.PostAsJsonAsync("/api/get-document", request);
        response.EnsureSuccessStatusCode();
        var returned = await response.Content.ReadFromJsonAsync<Document>();
        Assert.Equal("Test", returned?.Name);
    }
    
    [Fact]
    public async Task AddLesson_ReturnsBadRequest_WhenAddFails()
    {
        var lesson = new Lesson { IdAppointment = "A" };
        _repoMock.Setup(r => r.AddLesson(lesson, "1")).ReturnsAsync((Lesson?)null);
        var client = _factory.CreateClient();
        var req = new RequestLesson { IdSession = "s", Lesson = lesson };
        var resp = await client.PostAsJsonAsync("/api/add-lesson", req);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task DeleteLesson_ReturnsNotFound_WhenMissing()
    {
        _repoMock.Setup(r => r.GetLesson("A", "1")).ReturnsAsync((Lesson?)null);
        var client = _factory.CreateClient();
        var req = new RequestLesson { IdSession = "s", IdAppointement = "A", Lesson = new Lesson() };
        var resp = await client.PostAsJsonAsync("/api/delete-lesson", req);
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task DeleteLesson_Deletes_WhenFound()
    {
        var lesson = new Lesson { IdLesson = "L", IdAppointment = "A" };
        _repoMock.Setup(r => r.GetLesson("A", "1")).ReturnsAsync(lesson);
        _repoMock.Setup(r => r.DeleteLesson(lesson, "1")).ReturnsAsync(lesson);
        var client = _factory.CreateClient();
        var req = new RequestLesson { IdSession = "s", IdAppointement = "A", Lesson = lesson };
        var resp = await client.PostAsJsonAsync("/api/delete-lesson", req);
        resp.EnsureSuccessStatusCode();
        _repoMock.Verify(r => r.DeleteLesson(lesson, "1"), Times.Once);
    }
}


