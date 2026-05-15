using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services;

public class CartService
{
    private readonly ECommerceDbContext _context;

    public CartService(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var existingItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _context.Carts.Add(new Cart { UserID = userId, ProductID = productId, Quantity = quantity });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<Cart>> GetCartItemsAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Product)
            .Where(c => c.UserID == userId)
            .ToListAsync();
    }

    public async Task RemoveFromCartAsync(int cartId)
    {
        var item = await _context.Carts.FindAsync(cartId);
        if (item != null)
        {
            _context.Carts.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateQuantityAsync(int cartId, int quantity)
    {
        var item = await _context.Carts.FindAsync(cartId);
        if (item != null && quantity > 0)
        {
            item.Quantity = quantity;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = await _context.Carts.Where(c => c.UserID == userId).ToListAsync();
        _context.Carts.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetCartTotalAsync(int userId)
    {
        var items = await GetCartItemsAsync(userId);
        return items.Sum(i => i.Quantity * (i.Product?.Price ?? 0));
    }
}
