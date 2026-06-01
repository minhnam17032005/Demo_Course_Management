using ShopManagementAPI.Data;
using ShopManagementAPI.DTOs;
using ShopManagementAPI.DTOs.request;
using ShopManagementAPI.DTOs.response;
using ShopManagementAPI.Jwt;
using ShopManagementAPI.Exceptions;
using ShopManagementAPI.Models.Enum;
using ShopManagementAPI.Models;
using ShopManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Azure;
using Azure.Core;

namespace ShopManagementAPI.Services
{
    public class AuthService
    {
        private readonly UserRepository _repoUser;
        private readonly RoleRepository _repoRole;
        private readonly JwtService _jwtService;
        private readonly CurrentUserService _currentUser;
        private readonly IConfiguration _config;
        private readonly JwtBlacklistService _jwtBlacklist;


        public AuthService(UserRepository repoUser, RoleRepository repoRole,
            JwtService jwtService, CurrentUserService currentUser, IConfiguration config, JwtBlacklistService jwtBlacklist)
        {
            _repoUser = repoUser;
            _repoRole = repoRole;
            _jwtService = jwtService;
            _currentUser = currentUser;
            _config = config;
            _jwtBlacklist = jwtBlacklist;
        }
        public async Task<(LoginResponseDTO response, string refreshToken)> LoginAsync(LoginRequestDTO dto)
        {
            //check login và so sánh password 
            var user = await _repoUser.GetByUsernameWithRolesAsync(dto.Username);
            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);

            if (user == null||!isValid)
                throw new UnauthorizedException("Username hoặc password không hợp lệ");
            
            // check account lock 
            if (!user.IsActive)
                throw new UnauthorizedException("Tài khoản đã bị khóa.");

            //lấy ra access token
            var token = _jwtService.GenerateAccessToken(user);

            // tạo và gán refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiredAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpirationDays"])
            );
            await _repoUser.SaveAsync();

            var response = new LoginResponseDTO
            {
                User = new UserInfoDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Roles = user.UserRoles
                    .Select(x => new RoleItemDTO{
                        Id = x.RoleId,
                        Name = x.Role.Name
                    })
                    .ToList()
},
                AccessToken = token
            };

            //trả ra LoginResponseDTO và refreshToken cho controller 
            return (response, refreshToken);
        }

        public async Task<( RefreshResponseDTO response , string refreshToken) >RefreshTokenAsync(string refreshToken)
        {
            // tìm user theo refresh token
            var user = await _repoUser.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
                throw new UnauthorizedException("Phiên đăng nhập không hợp lệ.");

            if (!user.IsActive)
                throw new UnauthorizedException("Tài khoản đã bị khóa.");

            if (user.RefreshTokenExpiredAt < DateTime.UtcNow)
                throw new UnauthorizedException("Phiên đăng nhập đã hết hạn." );

            // tạo access token mới
            var accessToken = _jwtService.GenerateAccessToken(user);

            // rotate refresh token
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiredAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpirationDays"])
            );

            await _repoUser.SaveAsync();

            //trả ra accesstoken và refreshtoken mới 
            return (new RefreshResponseDTO { AccessToken = accessToken }, newRefreshToken);
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            var existsUsername = await _repoUser.IsUsernameExists(dto.Username);
            if (existsUsername)
                throw new ConflictException("Username already exists");

            var existsEmail = await _repoUser.IsEmailExists(dto.Email);
            if (existsEmail)
                throw new ConflictException("Email already exists");

            //mặc định role khi đăng ký là CUSTOMER
            var customerRole = await _repoRole.GetByNameAsync(RoleType.CUSTOMER);
            if (customerRole == null)
                throw new NotFoundException("Default role CUSTOMER not found");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                IsActive = true,
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = customerRole.Id
                    }
                }
            };
            await _repoUser.AddAsync(user);
            await _repoUser.SaveAsync();

            return new RegisterResponseDTO
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Message = "Register successfully"
            };
        }

        public async Task<UserProfileResponseDTO> GetProfileAsync()
        {
            // lấy userId từ JWT
            var userId = _currentUser.UserId;

            // query DB lấy dữ liệu user ,roles và permissions
            var user = await _repoUser.GetByIdWithRolesAsync(userId);
            var permissions = await _repoUser
                            .GetPermissionNamesAsync(userId);

            // token cũ / user bị xóa / user bị khóa
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("Phiên đăng nhập không hợp lệ hoặc tài khoản đã bị khóa.");
            }
            return new UserProfileResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Roles = user.UserRoles
                    .Select(x => new RoleItemDTO
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name
                    })
                    .ToList(),
                Permissions = permissions,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task LogoutAsync(string? refreshToken)
        {
            // blacklist access token
            /*if (!string.IsNullOrEmpty(jti) &&
                !string.IsNullOrEmpty(expClaim))
            {
                var expUnix = long.TryParse(expClaim);

                var expiredAt = DateTimeOffset
                    .FromUnixTimeSeconds(expUnix)
                    .UtcDateTime;

                var ttl = expiredAt - DateTime.UtcNow;

                if (ttl > TimeSpan.Zero){
                    await _jwtBlacklist.BlacklistTokenAsync(jti, ttl);
                }
            }*/
            //jti và expClaim lấy từ CurrentUser
            var jti = _currentUser.Jti;
            var expClaim = _currentUser.ExpiredAtString;

            if (long.TryParse(expClaim, out var expUnix))
            {
                var expiredAt = DateTimeOffset
                    .FromUnixTimeSeconds(expUnix)
                    .UtcDateTime;

                var ttl = expiredAt - DateTime.UtcNow;

                if (ttl > TimeSpan.Zero)
                {
                    await _jwtBlacklist
                        .BlacklistTokenAsync(jti, ttl);
                }
            }

            // remove refresh token
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = await _repoUser
                    .GetByRefreshTokenAsync(refreshToken);

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiredAt = null;

                    await _repoUser.SaveAsync();
                }
            }
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDTO request)
        {
            // lấy user id từ token
            var userId = _currentUser.UserId;
            // lấy jti access token
            var jti = _currentUser.Jti;
            // lấy ExpiredAt 
            var expClaim = _currentUser.ExpiredAtString;

            var user = await _repoUser.GetByIdAsync(userId) 
                ?? throw new NotFoundException(
                        "Người dùng không tồn tại."
                    );

            // check mật khẩu cũ
            bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash
            );
            if (!isCorrectPassword)
            {
                throw new BadRequestException(
                    "Mật khẩu hiện tại không đúng"
                );
            }
            // check xác nhận password
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new BadRequestException(
                    "Xác nhận mật khẩu không khớp"
                );
            }

            // check password mới khác password cũ
            bool isSameOldPassword = BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                user.PasswordHash
            );
            if (isSameOldPassword)
            {
                throw new BadRequestException(
                    "Mật khẩu mới không được trùng mật khẩu cũ"
                );
            }

            // hash password mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword
            );

            // revoke refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiredAt = null;
            
            await _repoUser.SaveAsync();
            // blacklist access token hiện tại
            if (long.TryParse(expClaim, out var expUnix))
            {
                var expiredAt = DateTimeOffset
                    .FromUnixTimeSeconds(expUnix)
                    .UtcDateTime;
                var ttl = expiredAt - DateTime.UtcNow;
                
                // token vẫn còn hạn mới blacklist
                if (ttl > TimeSpan.Zero)
                {
                    await _jwtBlacklist.BlacklistTokenAsync(
                        jti,
                        ttl
                    );
                }
            }
        }




    }
}
