using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniERP.Application.Exceptions;
using MiniERP.Application.Interfaces;
using MiniERP.Application.Requests.Categories;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            // Get all active categories to check for duplicate name (case-insensitive)
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            var duplicate = allCategories
                .Any(c => c.IsActive && 
                          c.Name.Trim().Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicate)
                throw new InvalidOperationException($"Category '{request.Name}' already exists.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category == null 
                ? null 
                : new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt
                };
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            // Check for duplicate name (excluding current category)
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            var duplicate = allCategories
                .Any(c => c.IsActive && c.Id != id && 
                          c.Name.Trim().Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicate)
                throw new InvalidOperationException($"Category '{request.Name}' already exists.");

            category.Name = request.Name.Trim();
            category.Description = request.Description?.Trim();

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            // Validación de negocio: no eliminar si tiene productos activos vinculados
            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var hasProducts = allProducts.Any(p => p.CategoryId == id && p.IsActive);

            if (hasProducts)
                throw new InvalidOperationException("Category has associated products.");

            // Soft Delete unificado
            category.IsActive = false;
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}