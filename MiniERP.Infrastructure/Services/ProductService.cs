using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniERP.Application.Interfaces;
using MiniERP.Application.Requests.Products;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var cat = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (cat == null || !cat.IsActive) throw new NotFoundException("Category not found.");
            var product = new Product { Name = request.Name.Trim(), Description = request.Description?.Trim(), Price = request.Price, Stock = request.Stock, CategoryId = request.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow };
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return new ProductResponse { Id = product.Id, Name = product.Name, Price = product.Price, Stock = product.Stock, CategoryId = product.CategoryId };
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            return p == null ? null : new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock, CategoryId = p.CategoryId };
        }

        public async Task<List<ProductResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Products.GetAllAsync();
            return list.Select(p => new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock, CategoryId = p.CategoryId }).ToList();
        }

        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            if (p == null) throw new NotFoundException("Product not found.");
            if (request.CategoryId != p.CategoryId)
            {
                var cat = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (cat == null || !cat.IsActive) throw new NotFoundException("Category not found.");
                p.CategoryId = request.CategoryId;
            }
            p.Name = request.Name.Trim();
            p.Description = request.Description?.Trim();
            p.Price = request.Price;
            p.Stock = request.Stock;
            await _unitOfWork.Products.UpdateAsync(p);
            await _unitOfWork.SaveChangesAsync();
            return new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock, CategoryId = p.CategoryId };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            if (p == null) throw new NotFoundException("Product not found.");
            p.IsActive = false;
            await _unitOfWork.Products.UpdateAsync(p);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}