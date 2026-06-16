using Application.Interfaces.Services;

namespace Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasher
{
    public string Hash(string password) => PasswordHasher.Hash(password);

    public bool Verify(string password, string hash) => PasswordHasher.Verify(password, hash);
}
