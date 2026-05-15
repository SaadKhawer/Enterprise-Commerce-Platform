// ============================================================
// PRODUCT CONTROLLER
// Endpoints: GetAll, GetByID, Search, Filter, Add, Update,
//            Delete, UpdateStock
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.DTOs;
using ShopWaveBlazor.Web.ECommerceAPI.Models;
using ShopWaveBlazor.Web.ECommerceAPI.Utils;

namespace ShopWaveBlazor.Web.ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ECommerceDbContext _db;

        public ProductController(ECommerceDbContext db) => _db = db;

        // ── GET /api/product ────────────────────────────────
        /// <summary>Get all active products (with optional filters via query string)</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetAll(
            [FromQuery] int?    categoryID  = null,
            [FromQuery] decimal? minPrice   = null,
            [FromQuery] decimal? maxPrice   = null,
            [FromQuery] bool    inStockOnly = false,
            [FromQuery] float?  minRating   = null,
            [FromQuery] string? search      = null,
            [FromQuery] int     page        = 1,
            [FromQuery] int     pageSize    = 20)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    (p.Category != null && p.Category.CategoryName.ToLower().Contains(term)));
            }

            // Filters
            if (categoryID.HasValue)
                query = query.Where(p => p.CategoryID == categoryID.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (inStockOnly)
                query = query.Where(p => p.StockQuantity > 0);

            if (minRating.HasValue)
                query = query.Where(p => (double)p.Rating >= minRating.Value);

            // Pagination
            int skip = (page - 1) * pageSize;
            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip(skip)
                .Take(pageSize)
                .Select(p => MapToDTO(p))
                .ToListAsync();

            return Ok(ApiResponse<List<ProductDTO>>.Ok(products));
        }

        // ── GET /api/product/{id} ───────────────────────────
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetByID(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id && p.IsActive);

            if (product == null)
                return NotFound(ApiResponse<ProductDTO>.Fail("Product not found."));

            return Ok(ApiResponse<ProductDTO>.Ok(MapToDTO(product)));
        }

        // ── GET /api/product/categories ────────────────────
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetCategories()
        {
            var cats = await _db.Categories
                .Select(c => new { c.CategoryID, c.CategoryName, c.Description })
                .ToListAsync();

            return Ok(ApiResponse<List<object>>.Ok(cats.Cast<object>().ToList()));
        }

        // ── POST /api/product ───────────────────────────────
        /// <summary>Add new product (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> Add(
            [FromBody] ProductCreateRequest req)
        {
            // Validate category exists
            bool catExists = await _db.Categories.AnyAsync(c => c.CategoryID == req.CategoryID);
            if (!catExists)
                return BadRequest(ApiResponse<ProductDTO>.Fail("Category not found."));

            if (req.Price <= 0)
                return BadRequest(ApiResponse<ProductDTO>.Fail("Price must be greater than 0."));

            if (req.StockQuantity < 0)
                return BadRequest(ApiResponse<ProductDTO>.Fail("Stock quantity cannot be negative."));

            var product = new Product
            {
                ProductName   = req.ProductName.Trim(),
                Description   = req.Description?.Trim(),
                Price         = req.Price,
                CategoryID    = req.CategoryID,
                ImageURL      = req.ImageURL?.Trim(),
                StockQuantity = req.StockQuantity,
                IsActive      = true,
                CreatedDate   = DateTime.Now
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // Load category for response
            await _db.Entry(product).Reference(p => p.Category).LoadAsync();

            return CreatedAtAction(nameof(GetByID),
                new { id = product.ProductID },
                ApiResponse<ProductDTO>.Ok(MapToDTO(product), "Product added successfully."));
        }

        // ── PUT /api/product/{id} ───────────────────────────
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> Update(
            int id, [FromBody] ProductUpdateRequest req)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound(ApiResponse<ProductDTO>.Fail("Product not found."));

            if (req.Price <= 0)
                return BadRequest(ApiResponse<ProductDTO>.Fail("Price must be greater than 0."));

            product.ProductName   = req.ProductName.Trim();
            product.Description   = req.Description?.Trim();
            product.Price         = req.Price;
            product.CategoryID    = req.CategoryID;
            product.ImageURL      = req.ImageURL?.Trim();
            product.StockQuantity = req.StockQuantity;
            product.IsActive      = req.IsActive;

            await _db.SaveChangesAsync();

            // Reload category
            await _db.Entry(product).Reference(p => p.Category).LoadAsync();

            return Ok(ApiResponse<ProductDTO>.Ok(MapToDTO(product), "Product updated successfully."));
        }

        // ── DELETE /api/product/{id} ────────────────────────
        /// <summary>Soft delete (set IsActive = false)</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(ApiResponse<string>.Fail("Product not found."));

            product.IsActive = false;   // Soft delete
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Product deleted successfully."));
        }

        // ── PATCH /api/product/{id}/stock ───────────────────
        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStock(
            int id, [FromBody] UpdateStockRequest req)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(ApiResponse<string>.Fail("Product not found."));

            if (req.Quantity < 0)
                return BadRequest(ApiResponse<string>.Fail("Stock quantity cannot be negative."));

            product.StockQuantity = req.Quantity;
            await _db.SaveChangesAsync();

            // Low stock notification for admin
            if (product.StockQuantity < 10)
            {
                var adminUsers = await _db.Users
                    .Where(u => u.UserType == "Admin" && u.IsActive)
                    .ToListAsync();

                foreach (var admin in adminUsers)
                {
                    _db.Notifications.Add(new Notification
                    {
                        UserID           = admin.UserID,
                        NotificationType = "LowStock",
                        Message          = $"Product \"{product.ProductName}\" is running low on stock ({product.StockQuantity} remaining)."
                    });
                }
                await _db.SaveChangesAsync();
            }

            return Ok(ApiResponse<string>.Ok($"Stock updated to {req.Quantity}."));
        }

        // ── GET /api/product/low-stock ──────────────────────
        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetLowStock(
            [FromQuery] int threshold = 10)
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.StockQuantity < threshold)
                .Select(p => MapToDTO(p))
                .ToListAsync();

            return Ok(ApiResponse<List<ProductDTO>>.Ok(products));
        }

        // ── Helper: Map Product → ProductDTO ────────────────
        private static ProductDTO MapToDTO(Product p) => new()
        {
            ProductID     = p.ProductID,
            ProductName   = p.ProductName,
            Description   = p.Description,
            Price         = p.Price,
            CategoryID    = p.CategoryID,
            CategoryName  = p.Category?.CategoryName ?? "",
            ImageURL      = p.ImageURL,
            StockQuantity = p.StockQuantity,
            IsActive      = p.IsActive,
            Rating        = p.Rating,
            TotalReviews  = p.TotalReviews
        };
    }

    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
    }
}
