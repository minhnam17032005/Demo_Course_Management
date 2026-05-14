    using System.Data;
using System.Data.Common;
using Demo_Course_Management.DTOs;
using Demo_Course_Management.DTOs.request;
using Demo_Course_Management.DTOs.response;
using Demo_Course_Management.Middleware;
using Demo_Course_Management.Models;
using Demo_Course_Management.Models.Enum;
using Demo_Course_Management.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demo_Course_Management.Services
{
    public class UserService
    {
        private readonly UserRepository _repoUser;
        private readonly UserRoleRepository _repoUserRole;
        private readonly RoleRepository _repoRole;
        private readonly OrderRepository _repoOrder;
      

        public UserService(UserRepository repoUser, RoleRepository repoRole, 
            UserRoleRepository repoUserRole, OrderRepository repoOrder)
        {
            _repoUser = repoUser;
            _repoRole = repoRole;
            _repoUserRole = repoUserRole;
            _repoOrder = repoOrder;
        }

        public async Task<UserResponseDTO> CreateAsync(CreateUserReqDTO dto)
        {
            // check username
            if (await _repoUser.IsUsernameExists(dto.Username))
                throw new BadRequestException("Username already exists");
            // check email
            if (await _repoUser.IsEmailExists(dto.Email))
                throw new BadRequestException("Email already exists");
            // check roleIds null / empty
            if (dto.RoleIds == null || !dto.RoleIds.Any())
                throw new BadRequestException("RoleIds is required");

            // validate roles
            var errors = new List<string>();

            var roleIds = dto.RoleIds.Distinct().ToList();

            var roles = await _repoRole.GetRolesByIdsAsync(roleIds);
            var foundIds = roles.Select(x => x.Id).ToHashSet();
            
            //custom errors list
            foreach (var roleId in roleIds)
            {
                var role = roles.FirstOrDefault(x => x.Id == roleId);
                if (role == null){
                    errors.Add($"RoleId {roleId} not found");
                    continue;
                }

                if (role.Name == RoleType.ADMIN){
                    errors.Add($"RoleId {roleId} is ADMIN and cannot be assigned");
                }
            }
            if (errors.Any())
                throw new BadRequestException(errors);

            // hash password
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                FullName = dto.FullName,
                Email = dto.Email,
                IsActive = true,

                UserRoles = roles.Select(role => new UserRole
                {
                    RoleId = role.Id
                }).ToList()
            };
            await _repoUser.AddAsync(user);
            await _repoUser.SaveAsync();

            // map response
            return MapToDTO(user);
        }

        //update profile or fullname
        public async Task<UserResponseDTO> ChangeProfileAsync(int id,ChangeProfileReqDTO dto)
        {
            var user = await _repoUser.GetByIdWithRolesAsync(id);

            if (user == null)
                throw new Exception("User not found");

            if (!user.IsActive)
                throw new Exception("User is inactive");

            user.FullName = dto.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            await _repoUser.SaveAsync();

            return MapToDTO(user);
        }

        public async Task<UserResponseDTO> AddRolesAsync(int userId, List<int> roleIds)
        {
            //vallidate
            var user = await _repoUser.GetByIdWithRolesAsync(userId)
                ?? throw new NotFoundException("Không tìm thấy người dùng.");
            if (!user.IsActive)
                throw new BadRequestException("Tài khoản người dùng đang bị khóa.");
            var newRoleIds = roleIds?.Distinct().ToList()
                ?? throw new BadRequestException("Danh sách vai trò không được để trống.");

            var roles = await _repoRole.GetRolesByIdsAsync(newRoleIds);
            var roleDict = roles.ToDictionary(x => x.Id);

            //errors list
            var errors = new List<string>();
            // role không tồn tại
            errors.AddRange(newRoleIds
                .Where(id => !roleDict.ContainsKey(id))
                .Select(id => $"Vai trò ID {id} không tồn tại."));
            // không cho gán ADMIN
            errors.AddRange(roleDict.Values
                .Where(r => r.Name == RoleType.ADMIN)
                .Select(r => "Không được phép gán quyền ADMIN cho người dùng."));

            if (errors.Any())
                throw new BadRequestException(errors);

            var currentIds = user.UserRoles.Select(x => x.RoleId).ToList();
            var addIds = newRoleIds.Except(currentIds).ToList();

            if (!addIds.Any())
                throw new BadRequestException("Không có vai trò mới để thêm.");

            user.UserRoles.AddRange(addIds.Select(id => new UserRole
            {
                UserId = user.Id,
                RoleId = id
            }));

            await _repoUser.SaveAsync();
            return MapToDTO(user);
        }

        public async Task<UserResponseDTO> RemoveRolesAsync(int userId, List<int> roleIds)
        {
            var user = await _repoUser.GetByIdWithRolesAsync(userId)
                ?? throw new NotFoundException("Không tìm thấy người dùng.");
            if (!user.IsActive)
                throw new BadRequestException("Tài khoản người dùng đang bị khóa.");
            var removeIds = roleIds?.Distinct().ToList()
                ?? throw new BadRequestException("Danh sách vai trò không được để trống.");

            //check tồn tại
            var existingRoles = user.UserRoles
                .Where(x => removeIds.Contains(x.RoleId))
                .ToList();

            if (!existingRoles.Any())
                throw new BadRequestException("Không tìm thấy vai trò nào tồn tại");

            // không cho xóa hết role
            var remainingCount = user.UserRoles.Count - existingRoles.Count;
            if (remainingCount <= 0)
                throw new BadRequestException("Người dùng phải có ít nhất một vai trò.");

            _repoUserRole.RemoveRange(existingRoles);

            await _repoUser.SaveAsync();
            return MapToDTO(user);
        }


        public async Task<List<UserResponseDTO>> GetAllAsync()
        {
            var users = await _repoUser.GetAllAsync();

            //sau có phân quyền sẽ phần quyền lại xem ai được xem gì 
            return users.Select(user => MapToDTO(user)).ToList();
        }

        public async Task<UserResponseDTO> GetByIdAsync(int id)
        {
            var user = await _repoUser.GetByIdWithRolesAsync(id);

            if (user == null)
                throw new NotFoundException("User not found");
            
            //sau có phân quyền sẽ phần quyền lại xem ai được xem gì 
            return MapToDTO(user);
        }

        public async Task<StatusResponseDTO> LockAsync(int id)
        {
            var user = await _repoUser.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");

            if (!user.IsActive)
                throw new BadRequestException("Tài khoản đã bị khóa.");

            // Không cho khóa Admin
            if (user.UserRoles.Any(r => r.Role.Name == RoleType.ADMIN))
                throw new BadRequestException("Không được khóa tài khoản Admin.");

            // Chỉ chặn nếu còn đơn Pending
            var hasPendingOrders = await _repoOrder.AnyPendingByUserIdAsync(id);

            if (hasPendingOrders)
                throw new ConflictException(
                    "Người dùng đang có đơn hàng chờ xử lý nên không thể khóa.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _repoUser.SaveAsync();
            return new StatusResponseDTO
            {
                IsActive = user.IsActive,
                Message = "User locked successfully"
            };
        }

        public async Task<StatusResponseDTO> UnlockAsync(int id)
        {
            var user = await _repoUser.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");

            if (user.IsActive)
                throw new BadRequestException("Tài khoản đang hoạt động.");

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _repoUser.SaveAsync();
            return new StatusResponseDTO
            {
                IsActive = user.IsActive,
                Message = "User unlocked successfully"
            };
        }
        public static UserResponseDTO MapToDTO(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles
                    .Select(x => new RoleItemDTO{
                        Id = x.Role.Id,
                        Name = x.Role.Name
                    })
                    .ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

    }
}
