using Samad.Application.Dtos;

namespace Samad.Application.IServices
{
    public interface IUserService
    {
        Task RegisterUser(RegisterDto dto);
        Task<LoginResultDto> LoginUser(LoginDto dto);
    }
}
