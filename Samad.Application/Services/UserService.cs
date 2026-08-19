using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public sealed class UserService(
    IRepository<User, int> userRepository,
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    IJwtTokenGenerator jwtTokenGenerator)
    : IUserService
    {
        public async Task RegisterUser(
            RegisterDto dto)
        {
            var email = dto.email
                .Trim()
                .ToLowerInvariant();

            var emailExists =
                await userRepository
                    .Query()
                    .AnyAsync(
                        x => x.Email == email);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "کاربری با این ایمیل قبلاً ثبت شده است.");
            }

            var nationalCodeExists =
                await userRepository
                    .Query()
                    .AnyAsync(
                        x => x.NationalCode == dto.nationalCode);

            if (nationalCodeExists)
            {
                throw new InvalidOperationException(
                    "این کد ملی قبلاً ثبت شده است.");
            }

            var user = new User
            {
                FirstName = dto.firstName.Trim(),
                LastName = dto.lastName.Trim(),
                PhoneNumber = dto.phoneNumber.Trim(),

                Email = email,

                NationalCode = dto.nationalCode.Trim(),

                BirthDate = dto.birthDate,

                RoleId = (int)UserRole.Applicant,

                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash =
                passwordService.Hash(dto.password);

            await userRepository.AddAsync(
                user);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<LoginResultDto> LoginUser(
            LoginDto dto)
        {
            var email = dto.email
                .Trim()
                .ToLowerInvariant();

            var user =
                await userRepository
                    .Query()
                    .Include(x => x.Role)
                    .SingleOrDefaultAsync(
                        x => x.Email == email);

            if (user is null)
            {
                throw new UnauthorizedAccessException(
                    "ایمیل یا رمز عبور اشتباه است.");
            }

            var validPassword =
                passwordService.Verify(
                    dto.password,
                    user.PasswordHash);

            if (!validPassword)
            {
                throw new UnauthorizedAccessException(
                    "ایمیل یا رمز عبور اشتباه است.");
            }

            var accessToken =
                jwtTokenGenerator.GenerateToken(user);

            var refreshToken =
                jwtTokenGenerator.GenerateRefreshToken();

            return new LoginResultDto(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                UserId: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                Role: user.Role.Title);
        }
    }
}
