using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Service.Cloudinary.Interfaces;
using Service.Database.Services;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Files;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Tests.Integration;

public class CloudControllerTests : IClassFixture<WebApplicationFactory<Service.Cloudinary.Program>>
{
    private readonly WebApplicationFactory<Service.Cloudinary.Program> _factory;
    private readonly Mock<ICloudRepository> _repoMock = new();

    private class ConstantHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public ConstantHandler(HttpResponseMessage response) { _response = response; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_response.StatusCode){Content = _response.Content});
        }
    }

    public CloudControllerTests(WebApplicationFactory<Service.Cloudinary.Program> factory)
    {
        var verifyResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new Dictionary<string,string>())
        };
        var handler = new ConstantHandler(verifyResponse);
        var verifyClient = new HttpClient(handler){BaseAddress = new Uri("http://localhost")};
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Url:ApiGateway","http://localhost"}}).Build();
        var verifyService = new VerifyDeleteService(verifyClient, config, new UserCloudService(new HttpClient(), config));

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICloudRepository));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton(_repoMock.Object);

                var verDesc = services.SingleOrDefault(d => d.ServiceType == typeof(VerifyDeleteService));
                if (verDesc != null) services.Remove(verDesc);
                services.AddSingleton(verifyService);

                var userDesc = services.SingleOrDefault(d => d.ServiceType == typeof(UserService));
                if (userDesc != null) services.Remove(userDesc);
                services.AddSingleton(new UserService(new HttpClient(), config));

                var cloudDesc = services.SingleOrDefault(d => d.ServiceType == typeof(Cloudinary));
                if (cloudDesc != null) services.Remove(cloudDesc);
                services.AddSingleton(new Cloudinary(new Account("c","k","s")));
            });
        });
    }

    [Fact]
    public async Task DeleteFile_ReturnsNotFound_WhenMissing()
    {
        _repoMock.Setup(r => r.GetFileAsyncByIdCloudinary("id")).ReturnsAsync((GetResourceResult?)null);
        var client = _factory.CreateClient();
        var req = new RequestLesson { Document = new Document{ IdCloudinary="id" }, Lesson = new Lesson{ Documents=new List<Document>() } };
        var resp = await client.PostAsJsonAsync("/api/delete-file", req);
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task DeleteFile_SkipsDeletion_WhenDocumentInUse()
    {
        _repoMock.Setup(r => r.GetFileAsyncByIdCloudinary("id")).ReturnsAsync(new GetResourceResult());
        var client = _factory.CreateClient();
        var req = new RequestLesson { Document = new Document{ IdCloudinary="id" }, Lesson = new Lesson{ Documents=new List<Document>{ new Document{ IdCloudinary="id" } } } };
        var resp = await client.PostAsJsonAsync("/api/delete-file", req);
        resp.EnsureSuccessStatusCode();
        _repoMock.Verify(r => r.DeleteFileAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteFile_Deletes_WhenUnused()
    {
        // recreate factory with verify service returning one doc
        var verifyResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new Dictionary<string,string>{{"1","1"}})
        };
        var handler = new ConstantHandler(verifyResponse);
        var verifyClient = new HttpClient(handler){BaseAddress = new Uri("http://localhost")};
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Url:ApiGateway","http://localhost"}}).Build();
        var verifyService = new VerifyDeleteService(verifyClient, config, new UserCloudService(new HttpClient(), config));
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var verDesc = services.SingleOrDefault(d => d.ServiceType == typeof(VerifyDeleteService));
                if (verDesc != null) services.Remove(verDesc);
                services.AddSingleton(verifyService);
            });
        });

        _repoMock.Setup(r => r.GetFileAsyncByIdCloudinary("id")).ReturnsAsync(new GetResourceResult());
        _repoMock.Setup(r => r.DeleteFileAsync("id")).ReturnsAsync(new DelResResult());
        var client = factory.CreateClient();
        var req = new RequestLesson { Document = new Document{ IdCloudinary="id" }, Lesson = new Lesson{ Documents=new List<Document>{ new Document{ IdCloudinary="id" } } } };
        var resp = await client.PostAsJsonAsync("/api/delete-file", req);
        resp.EnsureSuccessStatusCode();
        _repoMock.Verify(r => r.DeleteFileAsync("id"), Times.Once);
    }
}
