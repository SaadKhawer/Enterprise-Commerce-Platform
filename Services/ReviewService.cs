using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services
{
    public class ReviewService
    {
        private readonly IDbContextFactory<ECommerceDbContext> _dbFactory;

        public ReviewService(IDbContextFactory<ECommerceDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Review>> GetProductReviewsAsync(int productId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductID == productId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<bool> AddReviewAsync(Review review)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            
            review.ReviewDate = DateTime.Now;
            review.IsApproved = true; // Auto-approve for demo
            context.Reviews.Add(review);
            
            // Update Product Average Rating
            var product = await context.Products.FindAsync(review.ProductID);
            if (product != null)
            {
                var allReviews = await context.Reviews
                    .Where(r => r.ProductID == review.ProductID)
                    .ToListAsync();
                
                allReviews.Add(review); // Include current one
                
                product.TotalReviews = allReviews.Count;
                product.Rating = (decimal)allReviews.Average(r => r.Rating);
            }

            return await context.SaveChangesAsync() > 0;
        }
    }
}
