using System.Security.Claims;
using MaClasse.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MaClasse.Tests.Services;

public class CustomAuthenticationStateProviderTests
{
    private static CustomAuthenticationStateProvider CreateProvider()
    {
        var provider = new CustomAuthenticationStateProvider(
            new HttpContextAccessor(),
            new HttpClient(),
            null!,
            new ConfigurationBuilder().Build(),
            new ServiceCollection().BuildServiceProvider());
        return provider;
    }

    [Fact]
    public async Task NotifyUserAuthentication_SetsCurrentUser()
    {
        var provider = CreateProvider();
        var claims = new[] { new Claim(ClaimTypes.Name, "John") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

        await provider.NotifyUserAuthentication(principal);
        var state = await provider.GetAuthenticationStateAsync();

        Assert.Equal("John", state.User.Identity?.Name);
    }

    [Fact]
    public async Task NotifyUserLogout_ClearsCurrentUser()
    {
        var provider = CreateProvider();
        await provider.NotifyUserAuthentication(new ClaimsPrincipal(new ClaimsIdentity(new[]{new Claim(ClaimTypes.Name,"John")},"test")));

        await provider.NotifyUserLogout();
        var state = await provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity?.IsAuthenticated);
    }
}
