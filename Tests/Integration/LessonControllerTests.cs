using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Service.Database.Interfaces;
using MaClasse.Shared.Models.Files;
using Microsoft.Extensions.Configuration;
using Service.Database.Services;
using Xunit;

namespace Tests.Integration
{
    public class LessonControllerTests : IClassFixture<WebApplicationFactory<Service.Database.Program>>
    {
        private readonly WebApplicationFactory<Service.Database.Program> _factory;
        private readonly Mock<ILessonRepository> _repoMock = new();

        public LessonControllerTests(WebApplicationFactory<Service.Database.Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ILessonRepository));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddSingleton(_repoMock.Object);
                    // ensure UserService can be resolved even if not used
                    services.AddSingleton<UserService>(new UserService(new HttpClient(), new ConfigurationBuilder().Build()));                });
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
    }
}
