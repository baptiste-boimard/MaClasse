using Service.OAuth.Database;

namespace Service.OAuth.Repositories;

public class AuthRepository
{
    private readonly PostgresDbContext _postgresDbContext;

    public AuthRepository(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<> SignUpUser()
    {
        
    }
}