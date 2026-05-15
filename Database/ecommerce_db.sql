-- ============================================================
-- ADVANCED E-COMMERCE SYSTEM - SQL Server Database Script
-- Academic/Learning Project
-- ============================================================

USE master;
GO

-- Drop database if it exists (for clean setup)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    ALTER DATABASE ECommerceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDB;
END
GO

-- Create database
CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

-- ============================================================
-- TABLE CREATION (in dependency order)
-- ============================================================

-- 1. USERS TABLE
CREATE TABLE Users (
    UserID       INT           PRIMARY KEY IDENTITY(1,1),
    Username     VARCHAR(50)   NOT NULL UNIQUE,
    Password     VARCHAR(255)  NOT NULL,           -- Stores hashed password
    Email        VARCHAR(100)  NOT NULL UNIQUE,
    FullName     VARCHAR(100)  NOT NULL,
    PhoneNumber  VARCHAR(20)   NULL,
    Address      TEXT          NULL,
    UserType     VARCHAR(20)   NOT NULL DEFAULT 'Customer'  -- 'Admin' or 'Customer'
                               CHECK (UserType IN ('Admin', 'Customer')),
    CreatedDate  DATETIME      NOT NULL DEFAULT GETDATE(),
    IsActive     BIT           NOT NULL DEFAULT 1
);
GO

-- 2. CATEGORIES TABLE
CREATE TABLE Categories (
    CategoryID    INT          PRIMARY KEY IDENTITY(1,1),
    CategoryName  VARCHAR(100) NOT NULL,
    Description   TEXT         NULL
);
GO

-- 3. PRODUCTS TABLE
CREATE TABLE Products (
    ProductID      INT            PRIMARY KEY IDENTITY(1,1),
    ProductName    VARCHAR(200)   NOT NULL,
    Description    TEXT           NULL,
    Price          DECIMAL(10,2)  NOT NULL CHECK (Price > 0),
    CategoryID     INT            NOT NULL,
    ImageURL       VARCHAR(500)   NULL,
    StockQuantity  INT            NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    IsActive       BIT            NOT NULL DEFAULT 1,
    CreatedDate    DATETIME       NOT NULL DEFAULT GETDATE(),
    Rating         DECIMAL(3,2)   NOT NULL DEFAULT 0.00,  -- Average rating (0.00 - 5.00)
    TotalReviews   INT            NOT NULL DEFAULT 0,

    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID)
        REFERENCES Categories(CategoryID)
);
GO

-- 4. CART TABLE
CREATE TABLE Cart (
    CartID      INT       PRIMARY KEY IDENTITY(1,1),
    UserID      INT       NOT NULL,
    ProductID   INT       NOT NULL,
    Quantity    INT       NOT NULL CHECK (Quantity > 0),
    AddedDate   DATETIME  NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Cart_Users    FOREIGN KEY (UserID)    REFERENCES Users(UserID),
    CONSTRAINT FK_Cart_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    -- Prevent duplicate cart entries for same product
    CONSTRAINT UQ_Cart_UserProduct UNIQUE (UserID, ProductID)
);
GO

-- 5. ORDERS TABLE
CREATE TABLE Orders (
    OrderID          INT            PRIMARY KEY IDENTITY(1,1),
    UserID           INT            NOT NULL,
    OrderDate        DATETIME       NOT NULL DEFAULT GETDATE(),
    TotalAmount      DECIMAL(10,2)  NOT NULL CHECK (TotalAmount >= 0),
    DeliveryType     VARCHAR(20)    NOT NULL DEFAULT 'Standard'
                                   CHECK (DeliveryType IN ('Standard', 'Express')),
    DeliveryCharges  DECIMAL(10,2)  NOT NULL DEFAULT 100.00,
    OrderStatus      VARCHAR(20)    NOT NULL DEFAULT 'Pending'
                                   CHECK (OrderStatus IN ('Pending','Confirmed','Shipped','Delivered','Cancelled')),
    ShippingAddress  TEXT           NOT NULL,
    PhoneNumber      VARCHAR(20)    NOT NULL,
    PaymentStatus    VARCHAR(20)    NOT NULL DEFAULT 'Pending'
                                   CHECK (PaymentStatus IN ('Pending','Completed','Failed')),

    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- 6. ORDER DETAILS TABLE
CREATE TABLE OrderDetails (
    OrderDetailID  INT            PRIMARY KEY IDENTITY(1,1),
    OrderID        INT            NOT NULL,
    ProductID      INT            NOT NULL,
    Quantity       INT            NOT NULL CHECK (Quantity > 0),
    UnitPrice      DECIMAL(10,2)  NOT NULL CHECK (UnitPrice > 0),
    Subtotal       AS (Quantity * UnitPrice) PERSISTED,  -- Computed column

    CONSTRAINT FK_OrderDetails_Orders   FOREIGN KEY (OrderID)   REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

-- 7. PAYMENTS TABLE
CREATE TABLE Payments (
    PaymentID      INT            PRIMARY KEY IDENTITY(1,1),
    OrderID        INT            NOT NULL,
    PaymentMethod  VARCHAR(20)    NOT NULL
                                  CHECK (PaymentMethod IN ('CreditCard','JazzCash','EasyPaisa','COD')),
    PaymentDate    DATETIME       NOT NULL DEFAULT GETDATE(),
    Amount         DECIMAL(10,2)  NOT NULL CHECK (Amount > 0),
    TransactionID  VARCHAR(100)   NULL,      -- NULL for COD, generated for others
    PaymentStatus  VARCHAR(20)    NOT NULL DEFAULT 'Pending'
                                  CHECK (PaymentStatus IN ('Success','Failed','Pending')),

    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
GO

-- 8. REVIEWS TABLE
CREATE TABLE Reviews (
    ReviewID    INT       PRIMARY KEY IDENTITY(1,1),
    ProductID   INT       NOT NULL,
    UserID      INT       NOT NULL,
    Rating      INT       NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    ReviewText  TEXT      NULL,
    ReviewDate  DATETIME  NOT NULL DEFAULT GETDATE(),
    IsApproved  BIT       NOT NULL DEFAULT 0,  -- Admin approves before showing

    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT FK_Reviews_Users    FOREIGN KEY (UserID)    REFERENCES Users(UserID),
    -- One review per user per product
    CONSTRAINT UQ_Reviews_UserProduct UNIQUE (UserID, ProductID)
);
GO

-- 9. NOTIFICATIONS TABLE
CREATE TABLE Notifications (
    NotificationID    INT          PRIMARY KEY IDENTITY(1,1),
    UserID            INT          NOT NULL,
    NotificationType  VARCHAR(50)  NOT NULL
                                   CHECK (NotificationType IN (
                                       'OrderConfirmed','PaymentSuccess',
                                       'Shipped','Delivered','LowStock','Cancelled'
                                   )),
    Message           TEXT         NOT NULL,
    IsRead            BIT          NOT NULL DEFAULT 0,
    CreatedDate       DATETIME     NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- 10. CHAT MESSAGES TABLE
CREATE TABLE ChatMessages (
    MessageID   INT          PRIMARY KEY IDENTITY(1,1),
    UserID      INT          NOT NULL,
    SenderType  VARCHAR(10)  NOT NULL CHECK (SenderType IN ('User', 'Support')),
    Message     TEXT         NOT NULL,
    SentDate    DATETIME     NOT NULL DEFAULT GETDATE(),
    IsRead      BIT          NOT NULL DEFAULT 0,

    CONSTRAINT FK_ChatMessages_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- ============================================================
-- INDEXES (improve query performance)
-- ============================================================

CREATE INDEX IX_Products_CategoryID   ON Products(CategoryID);
CREATE INDEX IX_Products_IsActive     ON Products(IsActive);
CREATE INDEX IX_Cart_UserID           ON Cart(UserID);
CREATE INDEX IX_Orders_UserID         ON Orders(UserID);
CREATE INDEX IX_Orders_OrderStatus    ON Orders(OrderStatus);
CREATE INDEX IX_OrderDetails_OrderID  ON OrderDetails(OrderID);
CREATE INDEX IX_Reviews_ProductID     ON Reviews(ProductID);
CREATE INDEX IX_Notifications_UserID  ON Notifications(UserID, IsRead);
CREATE INDEX IX_ChatMessages_UserID   ON ChatMessages(UserID);
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- SP: Recalculate product rating after a new review
CREATE PROCEDURE sp_UpdateProductRating
    @ProductID INT
AS
BEGIN
    UPDATE Products
    SET
        Rating       = (SELECT AVG(CAST(Rating AS DECIMAL(3,2))) FROM Reviews WHERE ProductID = @ProductID AND IsApproved = 1),
        TotalReviews = (SELECT COUNT(*) FROM Reviews WHERE ProductID = @ProductID AND IsApproved = 1)
    WHERE ProductID = @ProductID;
END;
GO

-- SP: Place an order (atomic transaction)
CREATE PROCEDURE sp_PlaceOrder
    @UserID          INT,
    @DeliveryType    VARCHAR(20),
    @ShippingAddress TEXT,
    @PhoneNumber     VARCHAR(20),
    @PaymentMethod   VARCHAR(20),
    @NewOrderID      INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @DeliveryCharges DECIMAL(10,2) = CASE WHEN @DeliveryType = 'Express' THEN 300.00 ELSE 100.00 END;

        -- Calculate cart total
        DECLARE @CartTotal DECIMAL(10,2);
        SELECT @CartTotal = SUM(p.Price * c.Quantity)
        FROM Cart c
        JOIN Products p ON c.ProductID = p.ProductID
        WHERE c.UserID = @UserID;

        DECLARE @TotalAmount DECIMAL(10,2) = @CartTotal + @DeliveryCharges;

        -- Insert order
        INSERT INTO Orders (UserID, TotalAmount, DeliveryType, DeliveryCharges, ShippingAddress, PhoneNumber, PaymentStatus)
        VALUES (@UserID, @TotalAmount, @DeliveryType, @DeliveryCharges, @ShippingAddress, @PhoneNumber,
                CASE WHEN @PaymentMethod = 'COD' THEN 'Pending' ELSE 'Completed' END);

        SET @NewOrderID = SCOPE_IDENTITY();

        -- Insert order details from cart
        INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
        SELECT @NewOrderID, c.ProductID, c.Quantity, p.Price
        FROM Cart c
        JOIN Products p ON c.ProductID = p.ProductID
        WHERE c.UserID = @UserID;

        -- Reduce stock quantities
        UPDATE p
        SET p.StockQuantity = p.StockQuantity - c.Quantity
        FROM Products p
        JOIN Cart c ON p.ProductID = c.ProductID
        WHERE c.UserID = @UserID;

        -- Insert payment record
        INSERT INTO Payments (OrderID, PaymentMethod, Amount, TransactionID, PaymentStatus)
        VALUES (
            @NewOrderID,
            @PaymentMethod,
            @TotalAmount,
            CASE WHEN @PaymentMethod = 'COD' THEN NULL
                 ELSE 'TXN-' + CONVERT(VARCHAR, NEWID()) END,
            CASE WHEN @PaymentMethod = 'COD' THEN 'Pending' ELSE 'Success' END
        );

        -- Create order confirmation notification
        INSERT INTO Notifications (UserID, NotificationType, Message)
        VALUES (@UserID, 'OrderConfirmed',
                'Your order #' + CAST(@NewOrderID AS VARCHAR) + ' has been placed successfully!');

        -- Clear user's cart
        DELETE FROM Cart WHERE UserID = @UserID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- SP: Get dashboard analytics (Admin)
CREATE PROCEDURE sp_GetDashboardAnalytics
AS
BEGIN
    -- Summary cards
    SELECT
        (SELECT COUNT(*) FROM Orders)                                         AS TotalOrders,
        (SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders
         WHERE OrderStatus != 'Cancelled')                                    AS TotalSales,
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1)                   AS TotalProducts,
        (SELECT COUNT(*) FROM Products WHERE StockQuantity < 10 AND IsActive = 1) AS LowStockCount,
        (SELECT COUNT(*) FROM Users WHERE UserType = 'Customer')             AS TotalCustomers;

    -- Top 5 selling products
    SELECT TOP 5
        p.ProductID,
        p.ProductName,
        SUM(od.Quantity)    AS TotalSold,
        SUM(od.Subtotal)    AS TotalRevenue
    FROM OrderDetails od
    JOIN Products p ON od.ProductID = p.ProductID
    JOIN Orders o   ON od.OrderID   = o.OrderID
    WHERE o.OrderStatus != 'Cancelled'
    GROUP BY p.ProductID, p.ProductName
    ORDER BY TotalSold DESC;

    -- Recent 10 orders
    SELECT TOP 10
        o.OrderID,
        u.FullName,
        o.OrderDate,
        o.TotalAmount,
        o.OrderStatus,
        o.PaymentStatus
    FROM Orders o
    JOIN Users u ON o.UserID = u.UserID
    ORDER BY o.OrderDate DESC;

    -- Monthly sales (last 6 months)
    SELECT
        FORMAT(OrderDate, 'MMM yyyy')   AS MonthLabel,
        MONTH(OrderDate)                AS MonthNum,
        YEAR(OrderDate)                 AS YearNum,
        COUNT(*)                        AS OrderCount,
        SUM(TotalAmount)                AS MonthlyRevenue
    FROM Orders
    WHERE OrderDate >= DATEADD(MONTH, -6, GETDATE())
      AND OrderStatus != 'Cancelled'
    GROUP BY FORMAT(OrderDate, 'MMM yyyy'), MONTH(OrderDate), YEAR(OrderDate)
    ORDER BY YearNum, MonthNum;
END;
GO

-- ============================================================
-- SAMPLE DATA INSERTION
-- ============================================================

-- USERS (passwords below are SHA256 hashes)
-- admin123  => hashed value stored
-- customer123 => hashed value stored
-- For demo, we store plain-text note; replace with actual hash in C#

INSERT INTO Users (Username, Password, Email, FullName, PhoneNumber, Address, UserType) VALUES
('admin',     'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f',  -- admin123
 'admin@ecommerce.com',    'System Administrator', '03001234567',
 'Admin Office, Blue Area, Islamabad', 'Admin'),

('customer',  '9afe9c00c2993c5571b2c5a40d3c3a90c0c99c04f87fe2d547f7c4069b7f6eb6',  -- customer123
 'customer@gmail.com',     'Ali Hassan',           '03211234567',
 'House #12, Street 5, G-9, Islamabad', 'Customer'),

('sara_khan', '9afe9c00c2993c5571b2c5a40d3c3a90c0c99c04f87fe2d547f7c4069b7f6eb6',
 'sara@gmail.com',         'Sara Khan',            '03331234567',
 'Flat 3B, DHA Phase 5, Lahore', 'Customer'),

('ahmed_r',   '9afe9c00c2993c5571b2c5a40d3c3a90c0c99c04f87fe2d547f7c4069b7f6eb6',
 'ahmed@gmail.com',        'Ahmed Raza',           '03451234567',
 'Plot 22, Gulshan-e-Iqbal, Karachi', 'Customer');
GO

-- CATEGORIES
INSERT INTO Categories (CategoryName, Description) VALUES
('Electronics',   'Mobile phones, laptops, tablets, accessories and gadgets'),
('Clothing',      'Men, women and kids fashion, kurtas, jeans, shirts'),
('Books',         'Academic, fiction, self-help and Islamic books'),
('Home & Kitchen','Furniture, cookware, appliances and home decor'),
('Sports',        'Cricket, football, gym equipment and outdoor gear');
GO

-- PRODUCTS (20 products across categories)
INSERT INTO Products (ProductName, Description, Price, CategoryID, ImageURL, StockQuantity, Rating, TotalReviews) VALUES
-- Electronics (CategoryID = 1)
('Samsung Galaxy A54',
 'Samsung Galaxy A54 5G with 6.4" Super AMOLED display, 50MP camera, 5000mAh battery.',
 79999.00, 1, 'images/samsung-a54.jpg', 25, 4.50, 120),

('Apple AirPods Pro 2nd Gen',
 'Active Noise Cancellation, Adaptive Transparency, Personalized Spatial Audio with MagSafe charging case.',
 49999.00, 1, 'images/airpods-pro.jpg', 15, 4.80, 85),

('Lenovo IdeaPad Gaming 3',
 '15.6" FHD IPS, AMD Ryzen 5, 8GB RAM, 512GB SSD, NVIDIA GTX 1650 4GB, Windows 11.',
 134999.00, 1, 'images/lenovo-ideapad.jpg', 8, 4.30, 42),

('Xiaomi Redmi Buds 4 Active',
 'Wireless earbuds with 30h battery life, IP54 water resistance, fast charging.',
 3999.00, 1, 'images/redmi-buds.jpg', 50, 4.10, 200),

('Power Bank 20000mAh',
 'Fast charging 20W PD power bank with dual USB-A + USB-C output, LED display.',
 3499.00, 1, 'images/powerbank.jpg', 0, 3.80, 65),   -- Out of Stock

-- Clothing (CategoryID = 2)
('Bonanza Kurta Shalwar Set',
 'Embroidered lawn kurta shalwar with fine stitching, available in multiple colors.',
 4500.00, 2, 'images/kurta-set.jpg', 30, 4.60, 88),

('Levi''s 511 Slim Jeans',
 'Classic slim fit denim jeans for men, stretch fabric, multiple washes available.',
 6999.00, 2, 'images/levis-511.jpg', 20, 4.40, 55),

('Women''s Printed Lawn Suit (3pc)',
 'Unstitched 3-piece digital printed lawn suit with embroidered dupatta.',
 2200.00, 2, 'images/lawn-suit.jpg', 45, 4.20, 110),

('Nike Dri-FIT Running T-Shirt',
 'Moisture-wicking fabric, lightweight, breathable. Ideal for sports and gym.',
 3999.00, 2, 'images/nike-tshirt.jpg', 35, 4.70, 72),

('Puma Sport Sneakers',
 'Lightweight running shoes with cushioned sole, mesh upper, anti-slip grip.',
 8999.00, 2, 'images/puma-sneakers.jpg', 12, 4.30, 38),

-- Books (CategoryID = 3)
('Atomic Habits by James Clear',
 'An Easy & Proven Way to Build Good Habits & Break Bad Ones. Bestselling self-help book.',
 1499.00, 3, 'images/atomic-habits.jpg', 60, 4.90, 320),

('Riyazul Saliheen (Urdu)',
 'Classic collection of Hadith with Urdu translation and commentary. Hardcover edition.',
 899.00, 3, 'images/riyazul-saliheen.jpg', 40, 5.00, 150),

('Deep Work by Cal Newport',
 'Rules for Focused Success in a Distracted World. Essential for students and professionals.',
 1299.00, 3, 'images/deep-work.jpg', 25, 4.70, 180),

('O Level Pakistan Studies Guide',
 'Comprehensive Cambridge O Level Pakistan Studies revision guide with past papers.',
 1100.00, 3, 'images/o-level-pk.jpg', 0, 4.20, 60),   -- Out of Stock

-- Home & Kitchen (CategoryID = 4)
('Dawlance Microwave 25L',
 '25 Litre solo microwave oven with 5 power levels, child lock, 1 year warranty.',
 18999.00, 4, 'images/dawlance-microwave.jpg', 10, 4.40, 45),

('Stainless Steel Cookware Set 5pc',
 '5-piece non-stick stainless steel pots and pans set with glass lids, induction compatible.',
 6500.00, 4, 'images/cookware-set.jpg', 18, 4.50, 90),

('Anex Room Heater',
 '2000W quartz room heater with 2 heat settings, safety tip-over switch, cool-touch body.',
 4999.00, 4, 'images/anex-heater.jpg', 22, 3.90, 30),

-- Sports (CategoryID = 5)
('Gray-Nicolls Cricket Bat (English Willow)',
 'Grade 2 English Willow cricket bat, full size, pre-knocked, with anti-scuff sheet.',
 12999.00, 5, 'images/cricket-bat.jpg', 7, 4.60, 28),

('Adidas Football (Size 5)',
 'Official weight synthetic football, machine stitched, TPU material, size 5.',
 2999.00, 5, 'images/adidas-football.jpg', 35, 4.30, 55),

('Gym Dumbbell Set 20kg',
 'Adjustable rubber-coated dumbbell set 20kg (2x10kg), with storage rack included.',
 8500.00, 5, 'images/dumbbell-set.jpg', 14, 4.50, 40);
GO

-- SAMPLE ORDERS
INSERT INTO Orders (UserID, TotalAmount, DeliveryType, DeliveryCharges, OrderStatus, ShippingAddress, PhoneNumber, PaymentStatus) VALUES
(2, 84598.00, 'Express', 300.00, 'Delivered',  'House #12, Street 5, G-9, Islamabad', '03211234567', 'Completed'),
(2,  5599.00, 'Standard', 100.00, 'Shipped',   'House #12, Street 5, G-9, Islamabad', '03211234567', 'Completed'),
(3, 14099.00, 'Standard', 100.00, 'Confirmed', 'Flat 3B, DHA Phase 5, Lahore',        '03331234567', 'Completed'),
(4,  9099.00, 'Express',  300.00, 'Pending',   'Plot 22, Gulshan-e-Iqbal, Karachi',   '03451234567', 'Pending'),
(2,  2599.00, 'Standard', 100.00, 'Cancelled', 'House #12, Street 5, G-9, Islamabad', '03211234567', 'Failed');
GO

-- ORDER DETAILS
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES
(1, 1, 1, 79999.00),   -- Samsung A54
(1, 2, 1, 49999.00),   -- AirPods (Subtotal computed)
-- Note: TotalAmount = 79999 + 49999 + 300 delivery = 130298 (adjusted above for demo)
(2, 6, 1, 4500.00),    -- Kurta set
(2, 9, 1, 3999.00),    -- Nike T-shirt
(3, 3, 1, 134999.00),  -- Lenovo Laptop
(4, 18, 1, 12999.00),  -- Cricket bat
(4, 19, 2, 2999.00),   -- 2x Football (adjusted total above)
(5, 13, 2, 1299.00);   -- Deep Work x2
GO

-- PAYMENTS
INSERT INTO Payments (OrderID, PaymentMethod, Amount, TransactionID, PaymentStatus) VALUES
(1, 'CreditCard', 84598.00, 'TXN-CC-20240115001', 'Success'),
(2, 'JazzCash',    5599.00, 'TXN-JC-20240118002', 'Success'),
(3, 'EasyPaisa', 14099.00, 'TXN-EP-20240120003', 'Success'),
(4, 'COD',         9099.00, NULL,                 'Pending'),
(5, 'JazzCash',    2599.00, 'TXN-JC-20240122005', 'Failed');
GO

-- REVIEWS (approved)
INSERT INTO Reviews (ProductID, UserID, Rating, ReviewText, IsApproved) VALUES
(1,  2, 5, 'Excellent phone! Battery lasts all day and camera quality is superb. Highly recommended.',                1),
(1,  3, 4, 'Great value for money. Display is bright and smooth. Slight heating during gaming.',                      1),
(2,  2, 5, 'Sound quality is amazing. ANC works perfectly even in noisy environments. Worth every rupee.',            1),
(6,  3, 5, 'Stitching quality is fantastic. Material is very soft and comfortable. Will order again!',               1),
(11, 2, 5, 'Life-changing book. James Clear explains habits in a simple and practical way. A must-read.',            1),
(11, 4, 5, 'Best self-help book I have ever read. Already recommended it to 10 friends!',                            1),
(12, 3, 5, 'Alhamdulillah, excellent translation. Very helpful for daily reading.',                                  1),
(18, 4, 4, 'Good bat, well balanced. Needs a bit more knocking but overall great for the price.',                    1),
(9,  2, 5, 'Lightweight and breathable. Perfect for gym sessions. Nike quality is always reliable.',                  1),
(13, 3, 4, 'Great book about deep focus. Cal Newport writes very clearly. Recommended for students.',                1);
GO

-- NOTIFICATIONS
INSERT INTO Notifications (UserID, NotificationType, Message) VALUES
(2, 'OrderConfirmed', 'Your order #1 has been placed successfully!'),
(2, 'PaymentSuccess',  'Payment of Rs.84,598 received for order #1. Thank you!'),
(2, 'Delivered',       'Your order #1 has been delivered. Enjoy your purchase!'),
(2, 'OrderConfirmed', 'Your order #2 has been placed successfully!'),
(2, 'PaymentSuccess',  'Payment of Rs.5,599 received for order #2.'),
(2, 'Shipped',         'Your order #2 has been shipped! Expected delivery in 5-7 days.'),
(3, 'OrderConfirmed', 'Your order #3 has been placed successfully!'),
(3, 'PaymentSuccess',  'Payment of Rs.14,099 received for order #3.'),
(4, 'OrderConfirmed', 'Your order #4 has been placed. Pay on delivery.'),
(1, 'LowStock',        'Product "Gray-Nicolls Cricket Bat" is running low on stock (7 remaining).'),
(1, 'LowStock',        'Product "Lenovo IdeaPad Gaming 3" is running low on stock (8 remaining).');
GO

-- CHAT MESSAGES (sample support conversation)
INSERT INTO ChatMessages (UserID, SenderType, Message) VALUES
(2, 'User',    'Hi, I wanted to ask about my order #2. When will it be delivered?'),
(2, 'Support', 'Hello Ali! Your order #2 has been shipped and is on its way. Expected delivery in 3-5 business days.'),
(2, 'User',    'Great, thank you! Can I change the delivery address?'),
(2, 'Support', 'Sorry, the address cannot be changed once the order is shipped. Please contact us before shipment next time.'),
(3, 'User',    'Do you offer gift wrapping service?'),
(3, 'Support', 'Yes we do! Please mention it in the order notes at checkout. Additional Rs.150 will be charged.');
GO

-- CART (current items for demo customer)
INSERT INTO Cart (UserID, ProductID, Quantity) VALUES
(2, 4,  2),   -- 2x Redmi Buds
(2, 16, 1);   -- 1x Cookware Set
GO

-- ============================================================
-- USEFUL VIEWS
-- ============================================================

-- View: Product list with category name
CREATE VIEW vw_ProductsWithCategory AS
SELECT
    p.ProductID,
    p.ProductName,
    p.Description,
    p.Price,
    p.StockQuantity,
    p.Rating,
    p.TotalReviews,
    p.ImageURL,
    p.IsActive,
    c.CategoryID,
    c.CategoryName,
    CASE WHEN p.StockQuantity > 0 THEN 'In Stock' ELSE 'Out of Stock' END AS StockStatus
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID;
GO

-- View: Order summary with user info
CREATE VIEW vw_OrderSummary AS
SELECT
    o.OrderID,
    o.OrderDate,
    o.TotalAmount,
    o.DeliveryType,
    o.DeliveryCharges,
    o.OrderStatus,
    o.PaymentStatus,
    o.ShippingAddress,
    u.UserID,
    u.FullName,
    u.Email,
    u.PhoneNumber,
    COUNT(od.OrderDetailID) AS ItemCount
FROM Orders o
JOIN Users u        ON o.UserID  = u.UserID
JOIN OrderDetails od ON o.OrderID = od.OrderID
GROUP BY
    o.OrderID, o.OrderDate, o.TotalAmount, o.DeliveryType,
    o.DeliveryCharges, o.OrderStatus, o.PaymentStatus,
    o.ShippingAddress, u.UserID, u.FullName, u.Email, u.PhoneNumber;
GO

-- View: Approved reviews with user and product info
CREATE VIEW vw_ApprovedReviews AS
SELECT
    r.ReviewID,
    r.Rating,
    r.ReviewText,
    r.ReviewDate,
    u.FullName   AS ReviewerName,
    p.ProductName,
    p.ProductID
FROM Reviews r
JOIN Users    u ON r.UserID    = u.UserID
JOIN Products p ON r.ProductID = p.ProductID
WHERE r.IsApproved = 1;
GO

-- ============================================================
-- VERIFY DATA
-- ============================================================

SELECT 'Users'         AS TableName, COUNT(*) AS RecordCount FROM Users         UNION ALL
SELECT 'Categories',                 COUNT(*)               FROM Categories     UNION ALL
SELECT 'Products',                   COUNT(*)               FROM Products       UNION ALL
SELECT 'Orders',                     COUNT(*)               FROM Orders         UNION ALL
SELECT 'OrderDetails',               COUNT(*)               FROM OrderDetails   UNION ALL
SELECT 'Payments',                   COUNT(*)               FROM Payments       UNION ALL
SELECT 'Reviews',                    COUNT(*)               FROM Reviews        UNION ALL
SELECT 'Notifications',              COUNT(*)               FROM Notifications  UNION ALL
SELECT 'ChatMessages',               COUNT(*)               FROM ChatMessages   UNION ALL
SELECT 'Cart',                       COUNT(*)               FROM Cart;
GO

PRINT '============================================================';
PRINT ' ECommerceDB created successfully!';
PRINT ' Login credentials:';
PRINT '   Admin:    username=admin    | password=admin123';
PRINT '   Customer: username=customer | password=customer123';
PRINT '============================================================';
GO
