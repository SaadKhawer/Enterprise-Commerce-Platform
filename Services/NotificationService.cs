using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services;

public class NotificationService
{
    private readonly ECommerceDbContext _context;

    public NotificationService(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotificationAsync(int userId, string type, string message)
    {
        _context.Notifications.Add(new Notification
        {
            UserID = userId,
            NotificationType = type,
            Message = message,
            CreatedDate = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserID == userId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var n = await _context.Notifications.FindAsync(notificationId);
        if (n != null)
        {
            n.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
