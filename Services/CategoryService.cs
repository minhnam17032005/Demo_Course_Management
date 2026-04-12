using Demo_Course_Management.Data;
using Demo_Course_Management.DTOs.request;
using Demo_Course_Management.DTOs.response;
using Demo_Course_Management.Middleware;
using Demo_Course_Management.Models;
using Demo_Course_Management.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _repo;

        public CategoryService(CategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryResponseDTO> CreateAsync(CategoryRequestDTO dto)
        {
            // 1. check trùng name
            if (await _repo.ExistsByNameAsync(dto.Name))
                throw new BadRequestException("Category name already exists");

            // 2. tạo entity
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            // 3. lưu DB
            await _repo.AddAsync(category);
            await _repo.SaveChangesAsync();

            // 4. trả về DTO
            return MapToDTO(category);
        }

        public async Task<CategoryResponseDTO> UpdateAsync(int id, CategoryRequestDTO dto)
        {
            // 1. tìm category
            var category = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category not found");

            // 2. check trùng name (trừ chính nó)
            if (await _repo.ExistsByNameExcludeIdAsync(dto.Name, id))
                throw new BadRequestException("Category name already exists");

            // 3. update dữ liệu
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.UpdatedAt = DateTime.UtcNow;

            // 4. lưu DB
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            return MapToDTO(category);
        }

        public async Task<List<CategoryResponseDTO>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();

            return categories
                .Select(MapToDTO)
                .ToList();
        }

        public async Task<CategoryResponseDTO> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category not found");

            return MapToDTO(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category not found");

            _repo.Remove(category);
            await _repo.SaveChangesAsync();

            return true;
        }

        // helper mapping
        private static CategoryResponseDTO MapToDTO(Category c)
        {
            return new CategoryResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }


    }
}
