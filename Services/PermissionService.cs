using Demo_Course_Management.Data;
using Demo_Course_Management.DTOs.request;
using Demo_Course_Management.DTOs.response;
using Demo_Course_Management.Middleware;
using Demo_Course_Management.Models;
using Demo_Course_Management.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Services
{
    public class PermissionService
    {
        private readonly PermissionRepository _repo;

        public PermissionService(PermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PermissionResponseDTO>> GetAllAsync()
        {
            var permissions = await _repo.GetAllAsync();

            return permissions.Select(MapToDTO).ToList();
        }

        public async Task<PermissionResponseDTO> GetByIdAsync(int id)
        {
            var permission = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Permission not found");

            return MapToDTO(permission);
        }

        // MAPPING RIÊNG
        private static PermissionResponseDTO MapToDTO(Permission p)
        {
            return new PermissionResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ApiPath = p.ApiPath,
                Method = p.Method,
                Module = p.Module,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }

}
