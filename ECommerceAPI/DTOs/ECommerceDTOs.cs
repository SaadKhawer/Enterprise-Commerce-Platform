// ============================================================
// DTOs - Data Transfer Objects
// Request & Response models for API endpoints
// ============================================================

namespace ShopWaveBlazor.Web.ECommerceAPI.DTOs
{
    // ─── AUTH DTOs ──────────────────────────────────────────
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool   Success  { get; set; }
        public string Message  { get; set; } = string.Empty;
        public int    UserID   { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string Token    { get; set; } = string.Empty;   // JWT token
    }

    public class RegisterRequest
    {
        public string Username    { get; set; } = string.Empty;
        public string Password    { get; set; } = string.Empty;
        public string Email       { get; set; } = string.Empty;
        public string FullName    { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address     { get; set; } = string.Empty;
    }

    // ─── PRODUCT DTOs ───────────────────────────────────────
    public class ProductDTO
    {
        public int     ProductID     { get; set; }
        public string  ProductName   { get; set; } = string.Empty;
        public string? Description   { get; set; }
        public decimal Price         { get; set; }
        public int     CategoryID    { get; set; }
        public string  CategoryName  { get; set; } = string.Empty;
        public string? ImageURL      { get; set; }
        public int     StockQuantity { get; set; }
        public bool    IsActive      { get; set; }
        public decimal Rating        { get; set; }
        public int     TotalReviews  { get; set; }
        public string  StockStatus   => StockQuantity > 0 ? "In Stock" : "Out of Stock";
    }

    public class ProductCreateRequest
    {
        public string  ProductName   { get; set; } = string.Empty;
        public string? Description   { get; set; }
        public decimal Price         { get; set; }
        public int     CategoryID    { get; set; }
        public string? ImageURL      { get; set; }
        public int     StockQuantity { get; set; }
    }

    public class ProductUpdateRequest : ProductCreateRequest
    {
        public int  ProductID { get; set; }
        public bool IsActive  { get; set; } = true;
    }

    public class ProductFilterRequest
    {
        public int?    CategoryID  { get; set; }
        public decimal? MinPrice   { get; set; }
        public decimal? MaxPrice   { get; set; }
        public bool    InStockOnly { get; set; } = false;
        public float?  MinRating   { get; set; }
        public string? SearchTerm  { get; set; }
    }

    // ─── CART DTOs ──────────────────────────────────────────
    public class CartItemDTO
    {
        public int     CartID      { get; set; }
        public int     ProductID   { get; set; }
        public string  ProductName { get; set; } = string.Empty;
        public string? ImageURL    { get; set; }
        public decimal UnitPrice   { get; set; }
        public int     Quantity    { get; set; }
        public decimal Subtotal    => UnitPrice * Quantity;
        public int     MaxStock    { get; set; }
    }

    public class CartSummaryDTO
    {
        public List<CartItemDTO> Items           { get; set; } = new();
        public decimal           Subtotal        { get; set; }
        public decimal           DeliveryCharges { get; set; } = 100;
        public decimal           Total           => Subtotal + DeliveryCharges;
        public int               ItemCount       { get; set; }
    }

    public class AddToCartRequest
    {
        public int UserID    { get; set; }
        public int ProductID { get; set; }
        public int Quantity  { get; set; } = 1;
    }

    public class UpdateCartRequest
    {
        public int CartID   { get; set; }
        public int Quantity { get; set; }
    }

    // ─── ORDER DTOs ─────────────────────────────────────────
    public class PlaceOrderRequest
    {
        public int    UserID          { get; set; }
        public string DeliveryType    { get; set; } = "Standard";   // Standard | Express
        public string PaymentMethod   { get; set; } = "COD";
        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber     { get; set; } = string.Empty;

        // For Credit Card
        public string? CardNumber { get; set; }
        public string? CardCVV    { get; set; }
        public string? CardExpiry { get; set; }

        // For JazzCash / EasyPaisa
        public string? MobileAccount { get; set; }
    }

    public class OrderDTO
    {
        public int      OrderID         { get; set; }
        public DateTime OrderDate       { get; set; }
        public decimal  TotalAmount     { get; set; }
        public string   DeliveryType    { get; set; } = string.Empty;
        public decimal  DeliveryCharges { get; set; }
        public string   OrderStatus     { get; set; } = string.Empty;
        public string   PaymentStatus   { get; set; } = string.Empty;
        public string   ShippingAddress { get; set; } = string.Empty;
        public string   PhoneNumber     { get; set; } = string.Empty;
        public string   CustomerName    { get; set; } = string.Empty;
        public int      ItemCount       { get; set; }
        public List<OrderDetailDTO> Items { get; set; } = new();
    }

    public class OrderDetailDTO
    {
        public int     ProductID   { get; set; }
        public string  ProductName { get; set; } = string.Empty;
        public string? ImageURL    { get; set; }
        public int     Quantity    { get; set; }
        public decimal UnitPrice   { get; set; }
        public decimal Subtotal    { get; set; }
    }

    public class OrderTrackingDTO
    {
        public int    OrderID        { get; set; }
        public string CurrentStatus  { get; set; } = string.Empty;
        public string PaymentStatus  { get; set; } = string.Empty;
        public string PaymentMethod  { get; set; } = string.Empty;
        public string EstimatedDays  { get; set; } = string.Empty;
        public List<TrackingStep> Timeline { get; set; } = new();
    }

    public class TrackingStep
    {
        public string   Status      { get; set; } = string.Empty;
        public string   Label       { get; set; } = string.Empty;
        public bool     IsCompleted { get; set; }
        public bool     IsCurrent   { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    // ─── PAYMENT DTOs ───────────────────────────────────────
    public class PaymentDTO
    {
        public int      PaymentID     { get; set; }
        public int      OrderID       { get; set; }
        public string   PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate   { get; set; }
        public decimal  Amount        { get; set; }
        public string?  TransactionID { get; set; }
        public string   PaymentStatus { get; set; } = string.Empty;
    }

    // ─── REVIEW DTOs ────────────────────────────────────────
    public class ReviewDTO
    {
        public int      ReviewID     { get; set; }
        public int      ProductID    { get; set; }
        public string   ProductName  { get; set; } = string.Empty;
        public string   ReviewerName { get; set; } = string.Empty;
        public int      Rating       { get; set; }
        public string?  ReviewText   { get; set; }
        public DateTime ReviewDate   { get; set; }
        public bool     IsApproved   { get; set; }
    }

    public class AddReviewRequest
    {
        public int    ProductID  { get; set; }
        public int    UserID     { get; set; }
        public int    Rating     { get; set; }
        public string ReviewText { get; set; } = string.Empty;
    }

    // ─── NOTIFICATION DTOs ──────────────────────────────────
    public class NotificationDTO
    {
        public int      NotificationID   { get; set; }
        public string   NotificationType { get; set; } = string.Empty;
        public string   Message          { get; set; } = string.Empty;
        public bool     IsRead           { get; set; }
        public DateTime CreatedDate      { get; set; }
        public string   TimeAgo          { get; set; } = string.Empty;
    }

    // ─── CHAT DTOs ──────────────────────────────────────────
    public class ChatMessageDTO
    {
        public int      MessageID  { get; set; }
        public string   SenderType { get; set; } = string.Empty;
        public string   Message    { get; set; } = string.Empty;
        public DateTime SentDate   { get; set; }
        public bool     IsRead     { get; set; }
    }

    public class SendMessageRequest
    {
        public int    UserID     { get; set; }
        public string Message    { get; set; } = string.Empty;
        public string SenderType { get; set; } = "User";
    }

    // ─── DASHBOARD DTOs ─────────────────────────────────────
    public class DashboardDTO
    {
        public int     TotalOrders     { get; set; }
        public decimal TotalSales      { get; set; }
        public int     TotalProducts   { get; set; }
        public int     LowStockCount   { get; set; }
        public List<TopProductDTO>  TopProducts  { get; set; } = new();
        public List<OrderDTO>       RecentOrders { get; set; } = new();
        public List<MonthlySaleDTO> MonthlySales { get; set; } = new();
    }

    public class TopProductDTO
    {
        public int    ProductID   { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int    TotalSold   { get; set; }
        public decimal Revenue    { get; set; }
    }

    public class MonthlySaleDTO
    {
        public string  Month  { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int     Orders { get; set; }
    }

    // ─── GENERIC API RESPONSE ───────────────────────────────
    public class ApiResponse<T>
    {
        public bool   Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T?     Data    { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
