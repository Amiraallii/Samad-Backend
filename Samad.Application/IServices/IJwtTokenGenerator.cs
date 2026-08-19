using Samad.Domain.Entity;

namespace Samad.Application.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();

    }
}
