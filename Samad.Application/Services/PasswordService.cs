using Samad.Application.IServices;
using Samad.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Samad.Domain.Entity;
namespace Samad.Application.Services
{
    public sealed class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string Hash(string password)
        {
            var tempUser = new User();

            return _hasher.HashPassword(
                tempUser,
                password);
        }

        public bool Verify(
            string password,
            string passwordHash)
        {
            var tempUser = new User();

            var result = _hasher.VerifyHashedPassword(
                tempUser,
                passwordHash,
                password);

            return result is
                PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
