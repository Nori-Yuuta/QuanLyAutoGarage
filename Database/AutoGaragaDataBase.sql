/* =====================================================================
   AUTOGARAGE - DATABASE SCRIPT (SQL Server Express)
   Tương thích với SQL Server 2016+ / Express
   =====================================================================
   GHI CHÚ BẢO MẬT:
   - Cột Password hiện lưu plaintext để đồng bộ demo.
   - Khi triển khai thật: dùng BCRYPT / PBKDF2 / ASP.NET Identity.
   ===================================================================== */

USE master;
GO

IF DB_ID('AutoGarageDB') IS NOT NULL
BEGIN
    ALTER DATABASE AutoGarageDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AutoGarageDB;
END
GO

CREATE DATABASE AutoGarageDB;
GO
USE AutoGarageDB;
GO

/* =====================================================================
   1. USERS
   ===================================================================== */
CREATE TABLE dbo.Users
(
    UserId       INT IDENTITY(1,1) NOT NULL,
    Name         NVARCHAR(100)     NOT NULL,
    Email        NVARCHAR(200)     NOT NULL,
    Phone        NVARCHAR(20)      NULL,
    Password     NVARCHAR(255)     NOT NULL,   -- demo: plaintext
    Role         NVARCHAR(20)      NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'customer',
    CreatedAt    DATETIME2(0)      NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT CK_Users_Role CHECK (Role IN (N'admin', N'customer'))
);
GO
CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users(Email);
GO

/* =====================================================================
   2. CARS
   ===================================================================== */
CREATE TABLE dbo.Cars
(
    CarId          INT IDENTITY(1,1) NOT NULL,
    Name           NVARCHAR(200)  NOT NULL,
    Brand          NVARCHAR(50)   NOT NULL,
    Price          DECIMAL(18,0)  NOT NULL,      -- VND (không cần phần lẻ)
    ImageData      NVARCHAR(MAX)  NULL,          -- emoji hoặc 'data:image/...;base64,...'
    Year           INT            NULL,
    Engine         NVARCHAR(100)  NULL,
    Fuel           NVARCHAR(50)   NULL,
    Seats          INT            NULL,
    Transmission   NVARCHAR(100)  NULL,
    Origin         NVARCHAR(100)  NULL,
    Description    NVARCHAR(MAX)  NULL,
    CreatedAt      DATETIME2(0)   NOT NULL CONSTRAINT DF_Cars_CreatedAt DEFAULT SYSDATETIME(),
    UpdatedAt      DATETIME2(0)   NULL,
    CONSTRAINT PK_Cars PRIMARY KEY (CarId),
    CONSTRAINT CK_Cars_Price CHECK (Price >= 0)
);
GO
CREATE INDEX IX_Cars_Brand ON dbo.Cars(Brand);
GO

/* =====================================================================
   3. ACCESSORIES
   ===================================================================== */
CREATE TABLE dbo.Accessories
(
    AccessoryId    INT IDENTITY(1,1) NOT NULL,
    Name           NVARCHAR(200)  NOT NULL,
    Category       NVARCHAR(50)   NOT NULL,      -- Nội thất / Ngoại thất / An toàn / Vận hành
    Price          DECIMAL(18,0)  NOT NULL,
    PriceNote      NVARCHAR(50)   NULL,          -- VD: "/chiếc"
    ImageData      NVARCHAR(MAX)  NULL,
    Description    NVARCHAR(MAX)  NULL,
    Material       NVARCHAR(100)  NULL,
    Warranty       NVARCHAR(100)  NULL,
    CreatedAt      DATETIME2(0)   NOT NULL CONSTRAINT DF_Acc_CreatedAt DEFAULT SYSDATETIME(),
    UpdatedAt      DATETIME2(0)   NULL,
    CONSTRAINT PK_Accessories PRIMARY KEY (AccessoryId),
    CONSTRAINT CK_Accessories_Category CHECK (Category IN (N'Nội thất', N'Ngoại thất', N'An toàn', N'Vận hành')),
    CONSTRAINT CK_Accessories_Price CHECK (Price >= 0)
);
GO
CREATE INDEX IX_Acc_Category ON dbo.Accessories(Category);
GO

/* =====================================================================
   4. ACCESSORY <-> CAR COMPATIBILITY (many-to-many)
   ===================================================================== */
CREATE TABLE dbo.AccessoryCompatibility
(
    AccessoryId  INT NOT NULL,
    CarId        INT NOT NULL,
    CONSTRAINT PK_AccCompat PRIMARY KEY (AccessoryId, CarId),
    CONSTRAINT FK_AccCompat_Acc FOREIGN KEY (AccessoryId) REFERENCES dbo.Accessories(AccessoryId) ON DELETE CASCADE,
    CONSTRAINT FK_AccCompat_Car FOREIGN KEY (CarId)       REFERENCES dbo.Cars(CarId)       ON DELETE CASCADE
);
GO

/* =====================================================================
   5. NOTIFICATIONS
   ===================================================================== */
CREATE TABLE dbo.Notifications
(
    NotificationId INT IDENTITY(1,1) NOT NULL,
    Title          NVARCHAR(200) NOT NULL,
    Content        NVARCHAR(MAX) NOT NULL,
    [Date]         DATE          NOT NULL,
    CreatedAt      DATETIME2(0)  NOT NULL CONSTRAINT DF_Notif_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Notifications PRIMARY KEY (NotificationId)
);
GO

/* =====================================================================
   6. EVENTS (khuyến mãi)
   ===================================================================== */
CREATE TABLE dbo.Events
(
    EventId    INT IDENTITY(1,1) NOT NULL,
    Name       NVARCHAR(200) NOT NULL,
    Code       NVARCHAR(50)  NOT NULL,          -- mã giảm giá
    EndDate    DATE          NOT NULL,
    Status     NVARCHAR(50)  NOT NULL CONSTRAINT DF_Events_Status DEFAULT N'Đang diễn ra',
    CreatedAt  DATETIME2(0)  NOT NULL CONSTRAINT DF_Events_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Events PRIMARY KEY (EventId),
    CONSTRAINT CK_Events_Status CHECK (Status IN (N'Đang diễn ra', N'Sắp diễn ra', N'Đã kết thúc'))
);
GO
CREATE UNIQUE INDEX UX_Events_Code ON dbo.Events(Code);
GO

/* =====================================================================
   7. COUPONS (logic của checkout.js)
   ===================================================================== */
CREATE TABLE dbo.Coupons
(
    CouponId      INT IDENTITY(1,1) NOT NULL,
    Code          NVARCHAR(50)   NOT NULL,
    Type          NVARCHAR(20)   NOT NULL,      -- percent | shipping
    Value         DECIMAL(10,2)  NOT NULL,      -- % hoặc 100 cho shipping free
    MaxDiscount   DECIMAL(18,0)  NULL,
    Description   NVARCHAR(200)  NULL,
    IsActive      BIT            NOT NULL CONSTRAINT DF_Coupons_Active DEFAULT 1,
    ExpiresAt     DATETIME2(0)   NULL,
    CreatedAt     DATETIME2(0)   NOT NULL CONSTRAINT DF_Coupons_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Coupons PRIMARY KEY (CouponId),
    CONSTRAINT CK_Coupons_Type CHECK (Type IN (N'percent', N'shipping'))
);
GO
CREATE UNIQUE INDEX UX_Coupons_Code ON dbo.Coupons(Code);
GO

/* =====================================================================
   8. ORDERS
   ===================================================================== */
CREATE TABLE dbo.Orders
(
    OrderId         INT IDENTITY(1,1) NOT NULL,
    OrderCode       NVARCHAR(50)   NOT NULL,      -- VD: ORD1734567890123
    UserId          INT            NULL,          -- NULL nếu khách vãng lai
    RecipientName   NVARCHAR(100)  NOT NULL,
    RecipientPhone  NVARCHAR(20)   NOT NULL,
    RecipientEmail  NVARCHAR(200)  NULL,
    Province        NVARCHAR(100)  NOT NULL,
    District        NVARCHAR(100)  NOT NULL,
    Address         NVARCHAR(300)  NOT NULL,
    Note            NVARCHAR(500)  NULL,
    PaymentMethod   NVARCHAR(20)   NOT NULL,      -- cod | bank
    CouponCode      NVARCHAR(50)   NULL,
    Subtotal        DECIMAL(18,0)  NOT NULL,
    Shipping        DECIMAL(18,0)  NOT NULL,
    Discount        DECIMAL(18,0)  NOT NULL CONSTRAINT DF_Orders_Discount DEFAULT 0,
    Total           DECIMAL(18,0)  NOT NULL,
    Status          NVARCHAR(20)   NOT NULL CONSTRAINT DF_Orders_Status DEFAULT N'pending',
    CreatedAt       DATETIME2(0)   NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Orders PRIMARY KEY (OrderId),
    CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL,
    CONSTRAINT CK_Orders_Payment CHECK (PaymentMethod IN (N'cod', N'bank')),
    CONSTRAINT CK_Orders_Status  CHECK (Status IN (N'pending', N'processing', N'shipped', N'delivered', N'cancelled'))
);
GO
CREATE UNIQUE INDEX UX_Orders_OrderCode ON dbo.Orders(OrderCode);
CREATE INDEX IX_Orders_User   ON dbo.Orders(UserId);
CREATE INDEX IX_Orders_Date   ON dbo.Orders(CreatedAt DESC);
GO

/* =====================================================================
   9. ORDER ITEMS (snapshot – không FK bắt buộc để giữ lịch sử)
   ===================================================================== */
CREATE TABLE dbo.OrderItems
(
    OrderItemId   INT IDENTITY(1,1) NOT NULL,
    OrderId       INT            NOT NULL,
    AccessoryId   INT            NULL,          -- có thể NULL nếu sản phẩm đã xoá
    ItemName      NVARCHAR(200)  NOT NULL,
    ItemCategory  NVARCHAR(50)   NULL,
    ItemImage     NVARCHAR(MAX)  NULL,
    UnitPrice     DECIMAL(18,0)  NOT NULL,
    Quantity      INT            NOT NULL,
    LineTotal     AS (UnitPrice * Quantity) PERSISTED,
    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderItemId),
    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Acc   FOREIGN KEY (AccessoryId) REFERENCES dbo.Accessories(AccessoryId) ON DELETE SET NULL,
    CONSTRAINT CK_OrderItems_Qty   CHECK (Quantity > 0)
);
GO
CREATE INDEX IX_OrderItems_Order ON dbo.OrderItems(OrderId);
GO

/* =====================================================================
   10. CHAT MESSAGES
   ===================================================================== */
CREATE TABLE dbo.ChatMessages
(
    MessageId    INT IDENTITY(1,1) NOT NULL,
    UserId       INT            NULL,           -- NULL nếu khách vãng lai
    Sender       NVARCHAR(100)  NOT NULL,
    SenderEmail  NVARCHAR(200)  NULL,
    FromType     NVARCHAR(20)   NOT NULL,       -- customer | admin
    MessageText  NVARCHAR(MAX)  NOT NULL,
    SentAt       DATETIME2(0)   NOT NULL CONSTRAINT DF_Chat_SentAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_ChatMessages PRIMARY KEY (MessageId),
    CONSTRAINT FK_Chat_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL,
    CONSTRAINT CK_Chat_FromType CHECK (FromType IN (N'customer', N'admin'))
);
GO
CREATE INDEX IX_Chat_SentAt ON dbo.ChatMessages(SentAt DESC);
GO

/* =====================================================================
   SEED DATA
   ===================================================================== */

/* ---- 1. USERS: tài khoản admin mặc định (giống initAdminAccount) ---- */
INSERT INTO dbo.Users (Name, Email, Phone, Password, Role)
VALUES (N'Quản Trị Viên', N'admin@autogarage.vn', N'0999999999', N'admin123', N'admin');
GO

/* ---- 2. CARS ---- */
SET IDENTITY_INSERT dbo.Cars ON;
INSERT INTO dbo.Cars (CarId, Name, Brand, Price, ImageData, Year, Engine, Fuel, Seats, Transmission, Origin, Description) VALUES
(1, N'Toyota Camry 2024',          N'Toyota',        1150000000, N'🚗',  2024, N'2.5L Hybrid',             N'Xăng / Hybrid', 5, N'Tự động 8 cấp',  N'Nhật Bản', N'Sedan hạng D sang trọng, tiết kiệm nhiên liệu, an toàn 5 sao.'),
(2, N'Honda CR-V 2024',            N'Honda',         1050000000, N'🚙',  2024, N'1.5L Turbo',              N'Xăng',          7, N'CVT',            N'Nhật Bản', N'SUV 7 chỗ đa dụng, phù hợp gia đình, khoang hành lý rộng.'),
(3, N'Ford Ranger Wildtrak',       N'Ford',           920000000, N'🛻',  2024, N'2.0L Bi-Turbo',           N'Dầu',           5, N'Tự động 10 cấp', N'Mỹ',       N'Bán tải mạnh mẽ, khả năng off-road tốt, nội thất tiện nghi.'),
(4, N'Mazda CX-5 2024',            N'Mazda',          880000000, N'🚘',  2024, N'2.0L SkyActiv',           N'Xăng',          5, N'Tự động 6 cấp',  N'Nhật Bản', N'Crossover thiết kế KODO đẹp mắt, vận hành êm ái.'),
(5, N'BMW 320i 2024',              N'BMW',           1550000000, N'🏎️', 2024, N'2.0L TwinPower Turbo',    N'Xăng',          5, N'Tự động 8 cấp',  N'Đức',      N'Sedan thể thao hạng sang, cảm giác lái phấn khích.'),
(6, N'Mercedes C200 2024',         N'Mercedes-Benz', 1690000000, N'🚗',  2024, N'1.5L Turbo + EQ Boost',   N'Xăng',          5, N'Tự động 9 cấp',  N'Đức',      N'Sedan sang trọng, công nghệ MBUX hiện đại.');
SET IDENTITY_INSERT dbo.Cars OFF;
GO

/* ---- 3. ACCESSORIES ---- */
SET IDENTITY_INSERT dbo.Accessories ON;
INSERT INTO dbo.Accessories (AccessoryId, Name, Category, Price, PriceNote, ImageData, Description, Material, Warranty) VALUES
(101, N'Ghế Da Cao Cấp',           N'Nội thất',   15000000, NULL,        N'🪑', N'Ghế da bò thật, may thủ công, có sưởi và làm mát.',                N'Da bò thật',      N'24 tháng'),
(102, N'Màn hình Android 12"',     N'Nội thất',    8500000, NULL,        N'📺', N'Màn hình Android 12 inch, chip xử lý mạnh, tích hợp CarPlay/Android Auto.', N'Kính cường lực', N'12 tháng'),
(103, N'Thảm Lót Sàn 5D',          N'Nội thất',    2200000, NULL,        N'🧽', N'Thảm da lót sàn 5D chống thấm, vừa khít theo xe.',                 N'Da tổng hợp',     N'6 tháng'),
(104, N'Cánh Gió Thể Thao',        N'Ngoại thất',  5500000, NULL,        N'💨', N'Cánh gió carbon fiber, tăng tính khí động học và thẩm mỹ.',         N'Carbon Fiber',    N'12 tháng'),
(105, N'Camera 360 Độ',            N'An toàn',     7800000, NULL,        N'📹', N'Camera 360 độ, hiển thị góc nhìn toàn cảnh, hỗ trợ đỗ xe.',         N'Cảm biến CMOS',   N'18 tháng'),
(106, N'Lốp Michelin Pilot Sport', N'Vận hành',    4200000, N'/chiếc',   N'⚙️', N'Lốp thể thao cao cấp, bám đường tốt, giảm ồn.',                     N'Cao su tổng hợp', N'36 tháng'),
(107, N'Ắc Quy Khô 12V',           N'Vận hành',    2900000, NULL,        N'🔋', N'Ắc quy khô 12V 70Ah, tuổi thọ cao, khởi động mạnh.',               N'Chì canxi',       N'24 tháng'),
(108, N'Vô Lăng Bọc Da',           N'Nội thất',    1800000, NULL,        N'🎡', N'Vô lăng bọc da may tay, chống trượt, cảm giác cầm chắc.',           N'Da PU cao cấp',   N'6 tháng');
SET IDENTITY_INSERT dbo.Accessories OFF;
GO

/* ---- 4. ACCESSORY COMPATIBILITY ---- */
INSERT INTO dbo.AccessoryCompatibility (AccessoryId, CarId) VALUES
-- 101 Ghế Da
(101,1),(101,2),(101,4),(101,5),(101,6),
-- 102 Android
(102,1),(102,2),(102,3),(102,4),
-- 103 Thảm sàn
(103,1),(103,2),(103,3),(103,4),
-- 104 Cánh gió
(104,2),(104,3),(104,4),(104,5),
-- 105 Camera 360
(105,1),(105,2),(105,3),(105,4),(105,5),(105,6),
-- 106 Lốp
(106,1),(106,5),(106,6),
-- 107 Ắc quy
(107,1),(107,2),(107,3),(107,4),(107,5),(107,6),
-- 108 Vô lăng
(108,1),(108,2),(108,3),(108,4);
GO

/* ---- 5. COUPONS (đồng bộ với checkout.js) ---- */
INSERT INTO dbo.Coupons (Code, Type, Value, MaxDiscount, Description) VALUES
(N'SALE30',    N'percent',  30, 500000, N'Giảm 30% tối đa 500.000 ₫'),
(N'WELCOME10', N'percent',  10, 200000, N'Giảm 10% cho khách mới'),
(N'SHIPFREE',  N'shipping', 100, NULL,  N'Miễn phí vận chuyển');
GO

/* ---- 6. EVENTS demo ---- */
INSERT INTO dbo.Events (Name, Code, EndDate, Status) VALUES
(N'Flash Sale 30% Phụ Kiện', N'SALE30',   DATEADD(DAY, 3,  CAST(GETDATE() AS DATE)), N'Đang diễn ra'),
(N'Ưu Đãi Khách Mới',        N'WELCOME10',DATEADD(DAY, 30, CAST(GETDATE() AS DATE)), N'Đang diễn ra');
GO

/* ---- 7. NOTIFICATIONS demo ---- */
INSERT INTO dbo.Notifications (Title, Content, [Date]) VALUES
(N'Chào mừng đến AutoGarage', N'Hệ thống đã sẵn sàng phục vụ quý khách.', CAST(GETDATE() AS DATE));
GO

/* =====================================================================
   VIEWS hữu ích
   ===================================================================== */

-- View tổng quan đơn hàng (gọn cho dashboard)
CREATE VIEW dbo.vw_OrderSummary
AS
SELECT  o.OrderId,
        o.OrderCode,
        o.RecipientName,
        o.RecipientPhone,
        o.Total,
        o.Status,
        o.CreatedAt,
        COUNT(oi.OrderItemId) AS ItemCount
FROM    dbo.Orders o
LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.OrderId
GROUP BY o.OrderId, o.OrderCode, o.RecipientName, o.RecipientPhone, o.Total, o.Status, o.CreatedAt;
GO

-- View phụ kiện kèm danh sách xe tương thích (gộp chuỗi)
CREATE VIEW dbo.vw_AccessoryWithCompat
AS
SELECT  a.AccessoryId,
        a.Name,
        a.Category,
        a.Price,
        STUFF((SELECT N', ' + c.Name
               FROM   dbo.AccessoryCompatibility ac
               JOIN   dbo.Cars c ON c.CarId = ac.CarId
               WHERE  ac.AccessoryId = a.AccessoryId
               FOR XML PATH('')), 1, 2, N'') AS CompatList
FROM    dbo.Accessories a;
GO

-- View doanh thu theo ngày (cho Dashboard)
CREATE VIEW dbo.vw_RevenueByDay
AS
SELECT  CAST(CreatedAt AS DATE) AS [Date],
        COUNT(*)                AS OrderCount,
        SUM(Total)              AS Revenue
FROM    dbo.Orders
WHERE   Status <> N'cancelled'
GROUP BY CAST(CreatedAt AS DATE);
GO

/* =====================================================================
   STORED PROCEDURE mẫu: tạo đơn hàng (transaction-safe)
   ===================================================================== */
CREATE PROCEDURE dbo.sp_CreateOrder
    @UserId         INT = NULL,
    @OrderCode      NVARCHAR(50),
    @RecipientName  NVARCHAR(100),
    @RecipientPhone NVARCHAR(20),
    @RecipientEmail NVARCHAR(200) = NULL,
    @Province       NVARCHAR(100),
    @District       NVARCHAR(100),
    @Address        NVARCHAR(300),
    @Note           NVARCHAR(500) = NULL,
    @PaymentMethod  NVARCHAR(20),
    @CouponCode     NVARCHAR(50) = NULL,
    @Subtotal       DECIMAL(18,0),
    @Shipping       DECIMAL(18,0),
    @Discount       DECIMAL(18,0),
    @Total          DECIMAL(18,0),
    @NewOrderId     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.Orders
        (OrderCode, UserId, RecipientName, RecipientPhone, RecipientEmail,
         Province, District, Address, Note, PaymentMethod, CouponCode,
         Subtotal, Shipping, Discount, Total)
        VALUES
        (@OrderCode, @UserId, @RecipientName, @RecipientPhone, @RecipientEmail,
         @Province, @District, @Address, @Note, @PaymentMethod, @CouponCode,
         @Subtotal, @Shipping, @Discount, @Total);

        SET @NewOrderId = SCOPE_IDENTITY();

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

PRINT '✅ AutoGarageDB đã được tạo thành công.';
GO