// ============================================================
// MODELS - All Database Entity Classes
// ECommerce System - ASP.NET Core Web API
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopWaveBlazor.Web.ECommerceAPI.Models
{
    // ─── USER ───────────────────────────────────────────────
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Password { get; set; } = string.Empty;   // Hashed

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Required, StringLength(20)]
        public string UserType { get; set; } = "Customer";     // Admin | Customer

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Order>        Orders        { get; set; } = new List<Order>();
        public ICollection<Cart>         CartItems     { get; set; } = new List<Cart>();
        public ICollection<Review>       Reviews       { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ChatMessage>  ChatMessages  { get; set; } = new List<ChatMessage>();
    }

    // ─── CATEGORY ───────────────────────────────────────────
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // ─── PRODUCT ────────────────────────────────────────────
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required, StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [StringLength(500)]
        public string? ImageURL { get; set; }

        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0.00m;

        public int TotalReviews { get; set; } = 0;

        // Navigation
        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<Cart>        CartItems    { get; set; } = new List<Cart>();
        public ICollection<Review>      Reviews      { get; set; } = new List<Review>();
    }

    // ─── CART ───────────────────────────────────────────────
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("UserID")]
        public User? User { get; set; }

        [ForeignKey("ProductID")]
        public Product? Product { get; set; }
    }

    // ─── ORDER ──────────────────────────────────────────────
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required, StringLength(20)]
        public string DeliveryType { get; set; } = "Standard";   // Standard | Express

        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryCharges { get; set; } = 100.00m;

        [Required, StringLength(20)]
        public string OrderStatus { get; set; } = "Pending";
        // Pending | Confirmed | Shipped | Delivered | Cancelled

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";   // Pending | Completed | Failed

        // Navigation
        [ForeignKey("UserID")]
        public User? User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<Payment>     Payments     { get; set; } = new List<Payment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    // ─── ORDER DETAIL ───────────────────────────────────────
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        // Computed: Quantity * UnitPrice
        [NotMapped]
        public decimal Subtotal => Quantity * UnitPrice;

        // Navigation
        [ForeignKey("OrderID")]
        public Order? Order { get; set; }

        [ForeignKey("ProductID")]
        public Product? Product { get; set; }
    }

    // ─── PAYMENT ────────────────────────────────────────────
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required, StringLength(20)]
        public string PaymentMethod { get; set; } = string.Empty;
        // CreditCard | JazzCash | EasyPaisa | COD

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? TransactionID { get; set; }

        [Required, StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";   // Success | Failed | Pending

        // Navigation
        [ForeignKey("OrderID")]
        public Order? Order { get; set; }
    }

    // ─── REVIEW ─────────────────────────────────────────────
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        public string? ReviewText { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;

        // Navigation
        [ForeignKey("ProductID")]
        public Product? Product { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }
    }

    // ─── NOTIFICATION ───────────────────────────────────────
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, StringLength(50)]
        public string NotificationType { get; set; } = string.Empty;
        // OrderConfirmed | PaymentSuccess | Shipped | Delivered | LowStock | Cancelled

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }

    // ─── CHAT MESSAGE ───────────────────────────────────────
    public class ChatMessage
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, StringLength(10)]
        public string SenderType { get; set; } = "User";   // User | Support

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        // Navigation
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}
