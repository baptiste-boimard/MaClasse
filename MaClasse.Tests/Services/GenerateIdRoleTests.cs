using Service.OAuth.Interfaces;
using Service.OAuth.Service;
using Xunit;

namespace MaClasse.Tests.Services;

public class GenerateIdRoleTests
{
    private class FakeAuthRepository : IAuthRepository
    {
        private readonly Queue<bool> _responses;
        public List<string> CalledWith { get; } = new();

        public FakeAuthRepository(params bool[] responses)
        {
            _responses = new Queue<bool>(responses);
        }

        public Task<bool> CheckIdRole(string idRole)
        {
            CalledWith.Add(idRole);
            var value = _responses.Count > 0 ? _responses.Dequeue() : false;
            return Task.FromResult(value);
        }

        // Other interface members throw NotImplementedException
        public Task<UserProfile> GetOneUserByGoogleId(string googleId) => throw new NotImplementedException();
        public Task<UserProfile> AddUser(UserProfile user) => throw new NotImplementedException();
        public Task<UserProfile> UpdateUser(UserProfile user) => throw new NotImplementedException();
        public Task<List<Rattachment>> GetRattachmentByIdRole(string idRole) => throw new NotImplementedException();
        public Task<UserProfile> DeleteUser(UserProfile user) => throw new NotImplementedException();
        public Task<List<UserProfile>> GetUsersByIdRoles(List<Rattachment> listRattachments) => throw new NotImplementedException();
    }

    [Fact]
    public async Task GenerateIdAsync_IdAvailable_ReturnsId()
    {
        var repo = new FakeAuthRepository(false);
        var generator = new GenerateIdRole(repo);

        var id = await generator.GenerateIdAsync();

        Assert.NotNull(id);
        Assert.Equal(6, id.Length);
        Assert.Single(repo.CalledWith);
    }

    [Fact]
    public async Task GenerateIdAsync_RetriesUntilUnique()
    {
        var repo = new FakeAuthRepository(true, false);
        var generator = new GenerateIdRole(repo);

        var id = await generator.GenerateIdAsync();

        Assert.NotNull(id);
        Assert.Equal(6, id.Length);
        Assert.Equal(2, repo.CalledWith.Count);
    }
}
