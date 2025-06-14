using System;
using System.IO;
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

    [Fact]
    public async Task UploadFileAsync_UsesSlugifiedName_ForImage()
    {
        var cloudMock = new Mock<Cloudinary>(new Account("c","k","s"));
        ImageUploadParams? captured = null;
        cloudMock.Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>()))
            .Callback<ImageUploadParams>(p => captured = p)
            .ReturnsAsync(new ImageUploadResult { PublicId = "id" });

        var repo = new CloudRepository(cloudMock.Object, new SlugifyService());
        var stream = new MemoryStream(new byte[10]);
        var file = new FormFile(stream, 0, stream.Length, "data", "Été Image.PNG");
        await repo.UploadFileAsync(file, "user");

        Assert.NotNull(captured);
        Assert.Contains("ete_image", captured!.File!.FileName);
    }
}

