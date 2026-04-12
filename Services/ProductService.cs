using Demo_Course_Management.Data;
using Demo_Course_Management.DTOs.request;
using Demo_Course_Management.DTOs.response;
using Demo_Course_Management.Middleware;
using Demo_Course_Management.Models;
using Demo_Course_Management.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Demo_Course_Management.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repo;

        public ProductService(ProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProductResponseDTO> CreateAsync(ProductRequestDTO dto)
        {
            // 1. check category tồn tại
            var category = await _repo.GetCategoryByIdAsync(dto.CategoryId)
                ?? throw new NotFoundException("Category not found");

            // 2. check trùng product trong category
            if (await _repo.ExistsInCategoryAsync(dto.Name, dto.CategoryId))
                throw new BadRequestException("Product already exists in this category");

            // 3. tạo product
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId
            };

            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            return MapToDTO(product, category);
        }

        public async Task<ProductResponseDTO> UpdateAsync(int id, ProductRequestDTO dto)
        {
            var product = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Product not found");

            var category = await _repo.GetCategoryByIdAsync(dto.CategoryId)
                ?? throw new NotFoundException("Category not found");

            if (await _repo.ExistsInCategoryExcludeIdAsync(id, dto.Name, dto.CategoryId))
                throw new BadRequestException("Product already exists in this category");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            return MapToDTO(product, category);
        }

        public async Task<List<ProductResponseDTO>> GetAllAsync()
        {
            var products = await _repo.GetAllWithCategoryAsync();

            return products.Select(MapToDTO).ToList();
        }

        public async Task<ProductResponseDTO> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdWithCategoryAsync(id)
                ?? throw new NotFoundException("Product not found");

            return MapToDTO(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Product not found");

            _repo.Remove(product);
            await _repo.SaveChangesAsync();

            return true;
        }

        //MAPTODTO RIÊNG
        private static ProductResponseDTO MapToDTO(Product p)
        {
            return new ProductResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                //Lấy từ navigation property (p.Category)
                //Cần Include(p => p.Category) nếu không sẽ bị null
                CategoryName = p.Category?.Name,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        // overload cho create/update (có category riêng)
        private static ProductResponseDTO MapToDTO(Product p, Category c)
        {
            return new ProductResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                //Lấy trực tiếp từ parameter
                CategoryName = c.Name,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
