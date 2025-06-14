using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Service.Database.Services;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using Xunit;
using System.Collections.Generic;

namespace Tests.Unit;

public class VerifyDeleteServiceTests
{
    private class ConstantHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _factory;
        public ConstantHandler(Func<HttpRequestMessage, HttpResponseMessage> factory)
        {
            _factory = factory;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_factory(request));
        }
    }

    [Fact]
    public async Task VerifyDeleteFiles_ReturnsOnlyUnusedDocuments()
    {
        // handler returns success with dictionary count=1 for first call, empty for others
        int call = 0;
        var handler = new ConstantHandler(_ =>
        {
            call++;
            var content = call == 1
                ? JsonContent.Create(new Dictionary<string,string>{{"1","1"}})
                : JsonContent.Create(new Dictionary<string,string>());
            return new HttpResponseMessage(HttpStatusCode.OK){ Content = content };
        });
        var client = new HttpClient(handler){BaseAddress = new Uri("http://localhost")};
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Url:ApiGateway","http://localhost"}}).Build();
        var service = new VerifyDeleteService(client, config, new UserService(new HttpClient(), config));

        var docs = new List<Document>{ new Document{IdDocument="A"}, new Document{IdDocument="B"} };
        var request = new RequestLesson{ IdSession="s", Lesson = new Lesson{ Documents = docs } };

        var result = await service.VerifyDeleteFiles(request);
        Assert.Single(result);
        Assert.Equal("A", result[0].IdDocument);
        Assert.Equal(2, call);
    }

    [Fact]
    public async Task VerifyDeleteFiles_IgnoresFailures()
    {
        int calls = 0;
        var handler = new ConstantHandler(r => { calls++; return new HttpResponseMessage(HttpStatusCode.BadRequest); });
        var client = new HttpClient(handler){BaseAddress = new Uri("http://localhost")};
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Url:ApiGateway","http://localhost"}}).Build();
        var service = new VerifyDeleteService(client, config, new UserService(new HttpClient(), config));

        var docs = new List<Document>{ new Document{IdDocument="A"} };
        var request = new RequestLesson{ IdSession="s", Lesson = new Lesson{ Documents = docs } };

        var result = await service.VerifyDeleteFiles(request);
        Assert.Empty(result);
        Assert.Equal(1, calls);
    }
}
