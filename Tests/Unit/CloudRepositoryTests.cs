using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Moq;
using Service.Cloudinary.Repositories;
using Service.Database.Services;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace Tests.Unit;

public class CloudRepositoryTests
{
    [Fact]
    public async Task UploadFileAsync_Throws_OnInvalidFile()
    {
        var repo = new CloudRepository(new Mock<Cloudinary>(new Account("c","k","s")).Object, new SlugifyService());
        await Assert.ThrowsAsync<ArgumentException>(() => repo.UploadFileAsync(null!, "1"));
    }

    [Fact]
    public async Task UploadFileAsync_ReturnsNull_ForUnsupportedExtension()
    {
        var cloudMock = new Mock<Cloudinary>(new Account("c","k","s"));
        var file = new FormFile(new MemoryStream(new byte[1]), 0, 1, "data", "file.txt");
        var repo = new CloudRepository(cloudMock.Object, new SlugifyService());
        var result = await repo.UploadFileAsync(file, "1");
        Assert.Null(result);
    }
}

