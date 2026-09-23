using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Application.Tests.Fakes;

internal sealed class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = [];

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(Users.FirstOrDefault(user => user.Email == email));

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Users.FirstOrDefault(user => user.Id == id));

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => "hashed:" + password;

    public bool Verify(string password, string passwordHash) => passwordHash == "hashed:" + password;
}

internal sealed class FakeTokenService : ITokenService
{
    public string Create(User user) => "token:" + user.Id;
}

internal sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 9, 23, 12, 0, 0, TimeSpan.Zero);
}
