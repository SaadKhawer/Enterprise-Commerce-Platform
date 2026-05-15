using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services;

public class OrderService
{
    private readonly ECommerceDbContext _context;
    private readonly CartService _cartService;
    private readonly EmailService _emailService;

    public OrderService(ECommerceDbContext context, CartService cartService, EmailService emailService)
    {
        _context = context;
        _cartService = cartService;
        _emailService = emailService;
    }

    public async Task<Order?> PlaceOrderAsync(int userId, string deliveryType, string paymentMethod, string address, string phone, string? email = null)
    {
        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (!cartItems.Any()) return null;

        decimal subtotal = cartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0));
        decimal deliveryCharges = deliveryType == "Express" ? 300 : 100;
        decimal total = subtotal + deliveryCharges;

        var order = new Order
        {
            UserID = userId,
            OrderDate = DateTime.Now,
            TotalAmount = total,
            DeliveryType = deliveryType,
            DeliveryCharges = deliveryCharges,
            ShippingAddress = address,
            PhoneNumber = phone,
            OrderStatus = "Pending",
            PaymentStatus = "Pending",
            OrderDetails = cartItems.Select(ci => new OrderDetail
            {
                ProductID = ci.ProductID,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product?.Price ?? 0
            }).ToList()
        };

        _context.Orders.Add(order);

        // Update Stock
        foreach (var item in cartItems)
        {
            var product = await _context.Products.FindAsync(item.ProductID);
            if (product != null)
            {
                if (product.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Product '{product.ProductName}' has insufficient stock. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                }
                product.StockQuantity -= item.Quantity;
            }
        }

        try
        {
            await _context.SaveChangesAsync();
            await _cartService.ClearCartAsync(userId);

            // Create Payment record
            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentMethod = paymentMethod,
                Amount = total,
                PaymentStatus = paymentMethod == "COD" ? "Pending" : "Success",
                TransactionID = "TRX-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // 4. Send Email Notification
            try
            {
                // Use provided email, or fallback to user's registered email
                string? targetEmail = email;
                if (string.IsNullOrEmpty(targetEmail))
                {
                    var user = await _context.Users.FindAsync(userId);
                    targetEmail = user?.Email;
                }

                if (!string.IsNullOrEmpty(targetEmail))
                {
                    var fullOrder = await GetOrderDetailsAsync(order.OrderID);
                    if (fullOrder != null)
                    {
                        _ = Task.Run(async () => {
                            try {
                                await _emailService.SendOrderConfirmationEmailAsync(targetEmail, fullOrder);
                            } catch (Exception ex) {
                                Console.WriteLine($"Background Email Error: {ex.Message}");
                            }
                        });
                    }
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Order Confirmation Email Error: {ex.Message}");
            }

            return order;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DATABASE ERROR in PlaceOrder: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            throw;
        }
    }

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderDetailsAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);
    }
}
