using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;
using ShopWaveBlazor.Web.ECommerceAPI.Utils;

namespace ShopWaveBlazor.Web.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ECommerceDbContext context)
        {
            // 1. Ensure Admin User
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin" || u.Username == "saad");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Username = "admin",
                    Password = PasswordHelper.HashPassword("admin123"),
                    Email = "admin@shopwave.com",
                    FullName = "Admin User",
                    UserType = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(adminUser);
            }
            else if (adminUser.UserType != "Admin")
            {
                adminUser.UserType = "Admin";
            }

            // 2. Ensure Categories Exist (Smart Check)
            var existingHome = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Home & Kitchen");
            if (existingHome != null) {
                existingHome.CategoryName = "Home & Living";
                await context.SaveChangesAsync();
            }

            var categoryNames = new[] { "Electronics", "Clothing", "Home & Living", "Books" };
            foreach (var catName in categoryNames)
            {
                if (!await context.Categories.AnyAsync(c => c.CategoryName == catName))
                {
                    context.Categories.Add(new Category { CategoryName = catName, Description = $"{catName} products and gear" });
                }
            }
            await context.SaveChangesAsync();

            // 3. Ensure Products Exist (Smart Check)
            var categories = await context.Categories.ToListAsync();
            var electronicsId = categories.First(c => c.CategoryName == "Electronics").CategoryID;
            var clothingId    = categories.First(c => c.CategoryName == "Clothing").CategoryID;
            var homeId        = categories.First(c => c.CategoryName == "Home & Living").CategoryID;
            var booksId       = categories.First(c => c.CategoryName == "Books").CategoryID;

            var seedProducts = new List<Product>
            {
                new Product { ProductName = "iPhone 15 Pro Max", Price = 450000, CategoryID = electronicsId, StockQuantity = 100, Description = "Titanium design, A17 Pro chip, and most powerful iPhone camera ever.", Rating = 4.5m, TotalReviews = 234 },
                new Product { ProductName = "Samsung Galaxy S24 Ultra", Price = 285000, CategoryID = electronicsId, StockQuantity = 100, Description = "Built-in S Pen, 200MP camera, AI-powered Galaxy experience.", Rating = 4.5m, TotalReviews = 189 },
                new Product { ProductName = "MacBook Pro 14 inch", Price = 650000, CategoryID = electronicsId, StockQuantity = 100, Description = "M3 Pro chip, Liquid Retina XDR display, all-day battery life.", Rating = 4.8m, TotalReviews = 98 },
                new Product { ProductName = "Sony WH-1000XM5 Headphones", Price = 85000, CategoryID = electronicsId, StockQuantity = 100, Description = "Industry-leading noise cancellation, 30-hour battery, premium sound.", Rating = 4.7m, TotalReviews = 312 },
                new Product { ProductName = "iPad Pro 12.9 inch", Price = 380000, CategoryID = electronicsId, StockQuantity = 100, Description = "M2 chip, Liquid Retina XDR, works with Apple Pencil and Magic Keyboard.", Rating = 4.6m, TotalReviews = 145 },
                new Product { ProductName = "Samsung 55 inch 4K Smart TV", Price = 195000, CategoryID = electronicsId, StockQuantity = 100, Description = "Crystal UHD 4K, Smart TV with Alexa, HDR10+ support.", Rating = 4.4m, TotalReviews = 78 },
                new Product { ProductName = "Canon EOS R50 Camera", Price = 220000, CategoryID = electronicsId, StockQuantity = 100, Description = "24.2MP APS-C sensor, 4K video, perfect for content creators.", Rating = 4.6m, TotalReviews = 56 },
                new Product { ProductName = "Dell 27 inch Gaming Monitor", Price = 95000, CategoryID = electronicsId, StockQuantity = 100, Description = "165Hz refresh rate, 1ms response time, IPS panel, QHD resolution.", Rating = 4.5m, TotalReviews = 167 },
                new Product { ProductName = "Men's Premium Casual T-Shirt", Price = 2500, CategoryID = clothingId, StockQuantity = 100, Description = "100% cotton, breathable fabric, available in multiple colors.", Rating = 4.3m, TotalReviews = 523 },
                new Product { ProductName = "Women's Summer Floral Dress", Price = 4500, CategoryID = clothingId, StockQuantity = 100, Description = "Light floral pattern, perfect for summer, available in S/M/L/XL.", Rating = 4.5m, TotalReviews = 289 },
                new Product { ProductName = "Denim Jacket Unisex", Price = 6500, CategoryID = clothingId, StockQuantity = 100, Description = "Classic denim jacket, slim fit, vintage wash finish.", Rating = 4.4m, TotalReviews = 178 },
                new Product { ProductName = "Running Sports Sneakers", Price = 8500, CategoryID = clothingId, StockQuantity = 100, Description = "Lightweight, breathable mesh, cushioned sole for maximum comfort.", Rating = 4.6m, TotalReviews = 445 },
                new Product { ProductName = "Women's Leather Handbag", Price = 7500, CategoryID = clothingId, StockQuantity = 100, Description = "Genuine leather, multiple compartments, elegant design.", Rating = 4.4m, TotalReviews = 234 },
                new Product { ProductName = "Men's Slim Fit Formal Shirt", Price = 3200, CategoryID = clothingId, StockQuantity = 100, Description = "Premium cotton blend, wrinkle-resistant, perfect for office.", Rating = 4.3m, TotalReviews = 367 },
                new Product { ProductName = "Winter Puffer Jacket", Price = 12000, CategoryID = clothingId, StockQuantity = 100, Description = "Warm insulation, water-resistant outer shell, hood included.", Rating = 4.7m, TotalReviews = 156 },
                new Product { ProductName = "Atomic Habits - James Clear", Price = 1800, CategoryID = booksId, StockQuantity = 100, Description = "Build good habits, break bad ones. #1 New York Times bestseller.", Rating = 4.9m, TotalReviews = 892 },
                new Product { ProductName = "Rich Dad Poor Dad", Price = 1500, CategoryID = booksId, StockQuantity = 100, Description = "What the rich teach their kids about money that the poor do not.", Rating = 4.7m, TotalReviews = 743 },
                new Product { ProductName = "The Alchemist - Paulo Coelho", Price = 1200, CategoryID = booksId, StockQuantity = 100, Description = "A magical story about following your dreams and listening to your heart.", Rating = 4.8m, TotalReviews = 634 },
                new Product { ProductName = "Python Programming for Beginners", Price = 2200, CategoryID = booksId, StockQuantity = 100, Description = "Complete guide to Python, includes projects and exercises.", Rating = 4.5m, TotalReviews = 412 },
                new Product { ProductName = "The 7 Habits of Highly Effective People", Price = 1600, CategoryID = booksId, StockQuantity = 100, Description = "Powerful lessons in personal change by Stephen R. Covey.", Rating = 4.7m, TotalReviews = 567 },
                new Product { ProductName = "Ceramic Coffee Mug Set of 6", Price = 3500, CategoryID = homeId, StockQuantity = 100, Description = "Premium ceramic, dishwasher safe, beautiful minimalist design.", Rating = 4.5m, TotalReviews = 334 },
                new Product { ProductName = "LED Adjustable Desk Lamp", Price = 4500, CategoryID = homeId, StockQuantity = 100, Description = "5 color modes, touch control, USB charging port, eye-care technology.", Rating = 4.4m, TotalReviews = 223 },
                new Product { ProductName = "Smart Air Purifier", Price = 35000, CategoryID = homeId, StockQuantity = 100, Description = "HEPA filter, removes 99.9% pollutants, WiFi enabled, covers 500 sq ft.", Rating = 4.6m, TotalReviews = 89 },
                new Product { ProductName = "Luxury Throw Pillow Set of 4", Price = 5500, CategoryID = homeId, StockQuantity = 100, Description = "Soft velvet cover, hypoallergenic filling, elegant home decor.", Rating = 4.3m, TotalReviews = 178 },
                new Product { ProductName = "Stainless Steel Kitchen Knife Set", Price = 8500, CategoryID = homeId, StockQuantity = 100, Description = "Professional grade, 8-piece set, ergonomic handles, ultra-sharp blades.", Rating = 4.7m, TotalReviews = 267 },
                new Product { ProductName = "Aromatherapy Diffuser & Humidifier", Price = 6500, CategoryID = homeId, StockQuantity = 100, Description = "Essential oil diffuser, 7 LED colors, auto shut-off, 500ml capacity.", Rating = 4.5m, TotalReviews = 198 }
            };

            foreach (var p in seedProducts)
            {
                var existing = await context.Products.FirstOrDefaultAsync(x => x.ProductName == p.ProductName);
                if (existing == null)
                {
                    context.Products.Add(p);
                }
                else 
                {
                    existing.Description = p.Description;
                    existing.Price = p.Price;
                    existing.Rating = p.Rating;
                    existing.TotalReviews = p.TotalReviews;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
