using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;
using ShopWaveBlazor.Web.ECommerceAPI.Utils;

namespace ShopWaveBlazor.Web.Services;

public class AuthService
{
    private readonly IDbContextFactory<ECommerceDbContext> _dbFactory;

    public AuthService(IDbContextFactory<ECommerceDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public User? CurrentUser { get; private set; }

    public async Task<bool> LoginAsync(string username, string password)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        
        if (user != null)
        {
            bool isValid = PasswordHelper.VerifyPassword(password, user.Password) 
                        || PasswordHelper.VerifyLegacyPassword(password, user.Password)
                        || user.Password == password;

            if (isValid)
            {
                CurrentUser = user;
                return true;
            }
        }
        return false;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public async Task<bool> RegisterAsync(User user)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        if (await context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
            return false;

        user.Password = PasswordHelper.HashPassword(user.Password);
        user.CreatedDate = DateTime.Now;
        user.IsActive = true;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LoadUserAsync(int userId)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);
        if (user != null)
        {
            CurrentUser = user;
            return true;
        }
        return false;
    }

    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.UserType == "Admin";
}
