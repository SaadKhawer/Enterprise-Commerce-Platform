using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services;

public class ProductService
{
    private readonly ECommerceDbContext _context;

    public ProductService(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Include(p => p.Category).Where(p => p.IsActive).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);
    }

    public async Task<List<Product>> SearchProductsAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return await GetAllProductsAsync();
        
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && 
                       (p.ProductName.Contains(term) || 
                        (p.Description != null && p.Description.Contains(term)) || 
                        (p.Category != null && p.Category.CategoryName.Contains(term))))
            .ToListAsync();
    }

    public async Task<List<Product>> FilterProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, bool inStockOnly)
    {
        var query = _context.Products.Include(p => p.Category).Where(p => p.IsActive).AsQueryable();

        if (categoryId.HasValue && categoryId > 0)
            query = query.Where(p => p.CategoryID == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStockOnly)
            query = query.Where(p => p.StockQuantity > 0);

        return await query.ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantityChange)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        product.StockQuantity += quantityChange;
        await _context.SaveChangesAsync();
        return true;
    }
}
