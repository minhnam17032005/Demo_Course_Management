using Demo_Course_Management.DTOs.response;
using Demo_Course_Management.DTOs;
using Microsoft.EntityFrameworkCore;
using Demo_Course_Management.Data;
using Demo_Course_Management.Middleware;
using Demo_Course_Management.Models.Enum;
using Demo_Course_Management.Models;
using Demo_Course_Management.Repositories;

namespace Demo_Course_Management.Services
{
    public class RoleService
    {
        private readonly RoleRepository _repoRole;
        private readonly RolePermissionRepository _repoRolePermission;
        private readonly PermissionRepository _repoPermission;



        public RoleService(RoleRepository repoRole, RolePermissionRepository repoRolePermission, PermissionRepository repoPermission)
        {
            _repoRole = repoRole;
            _repoRolePermission = repoRolePermission;
            _repoPermission = repoPermission;
        }

        // ================= GET ALL =================
        public async Task<List<RoleResponseDTO>> GetAllAsync()
        {
            var roles = await _repoRole.GetAllWithPermissionsAsync();

            return roles.Select(MapToDTO).ToList();
        }

        // ================= GET BY ID =================
        public async Task<RoleResponseDTO> GetByIdAsync(int id)
        {
            var role = await _repoRole.GetByIdWithPermissionsAsync(id)
                ?? throw new NotFoundException("Role not found");

            return MapToDTO(role);
        }

        // ================= ADD PERMISSIONS =================
        public async Task<RolePermissionResponseDTO> AddPermissionsAsync(int roleId, List<int> permissionIds)
        {
            ValidateInput(permissionIds);
            var role = await _repoRole.FindByIdAsync(roleId)
                ?? throw new NotFoundException("Role not found");
            
            ValidateRole(role);
            //Lấy permission hợp lệ trong DB
            var validIds = await _repoPermission.GetValidPermissionIdsAsync(permissionIds);

            //Lấy permission đã tồn tại trong role
            var existingIds = await _repoPermission.GetExistingPermissionIdsAsync(roleId, validIds);

            //Xác định permission cần thêm
            var toAdd = validIds.Except(existingIds).ToList();

            //Xác định permission fail (không tồn tại hoặc đã có)
            var failedIds = permissionIds.Except(validIds).Union(existingIds).Distinct().ToList();

            //Nếu không có cái mới nào
            if (!toAdd.Any())
            {
                return new RolePermissionResponseDTO
                {
                    RoleId = roleId,
                    ProcessedIds = new List<int>(),
                    FailedIds = permissionIds,
                    Message = "No new permissions to add"
                };
            }

            //Insert mới
            _repoRolePermission.AddRolePermissions(
                toAdd.Select(id => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = id
                }).ToList()
            );

            await _repoRole.SaveChangesAsync();
            return new RolePermissionResponseDTO
            {
                RoleId = roleId,
                ProcessedIds = toAdd,
                FailedIds = failedIds,
                Message = "Permissions added successfully"
            };
        }

        // ================= REMOVE PERMISSIONS =================
        public async Task<RolePermissionResponseDTO> RemovePermissionsAsync(int roleId, List<int> permissionIds)
        {
            ValidateInput(permissionIds);
            var role = await _repoRole.FindByIdAsync(roleId)
                ?? throw new NotFoundException("Role not found");

            ValidateRole(role);
            //Lấy các mapping tồn tại trong DB
            var entities = await _repoRolePermission.GetRolePermissionsAsync(roleId, permissionIds);

            //Nếu không có cái nào tồn tạ
            if (!entities.Any())
            {
                return new RolePermissionResponseDTO
                {
                    RoleId = roleId,
                    ProcessedIds = new List<int>(),
                    FailedIds = permissionIds,
                    Message = "No permissions found in this role"
                };
            }

            //danh sách permission thực sự sẽ bị xóa
            var removedIds = entities.Select(x => x.PermissionId).ToList();

            //danh sách fail (không tồn tại trong role)
            var failedIds = permissionIds.Except(removedIds).ToList();

            //remove mapping
            _repoRolePermission.RemoveRolePermissions(entities);

            await _repoRole.SaveChangesAsync();
            return new RolePermissionResponseDTO
            {
                RoleId = roleId,
                ProcessedIds = removedIds,
                FailedIds = failedIds,
                Message = "Permissions removed successfully"
            };
        }

        // ================= MAPPER =================
        private static RoleResponseDTO MapToDTO(Role r)
        {
            return new RoleResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,

                Permissions = r.RolePermissions
                    .Select(rp => new PermissionItemDTO
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name
                    }).ToList(),

                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            };
        }

        // ================= VALIDATION =================
        // check input rỗng/null
        private static void ValidateInput(List<int> permissionIds)
        {
            if (permissionIds == null || !permissionIds.Any())
                throw new BadRequestException("PermissionIds is empty");
        }
        // check quyền được phép sửa
        private static void ValidateRole(Role role)
        {
            if (role.Name != RoleType.STAFF && role.Name != RoleType.CUSTOMER)
                throw new BadRequestException("Only STAFF and CUSTOMER can be modified");
        }
    }
}
