namespace AutoGarageMVC.Models;

using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Services;

public static class DbSeeder
{
    public static void Seed(AutoGarageDbContext db)
    {
        var hasher = new PasswordHasher();

        // 1. Seed Admin & Demo Customer
        if (!db.Users.Any(u => u.Role == "admin"))
        {
            db.Users.Add(new User
            {
                Name = "Quản Trị Viên",
                Email = "admin@autogarage.vn",
                Phone = "0999999999",
                PasswordHash = hasher.Hash("admin123"),
                Role = "admin"
            });
        }

        if (!db.Users.Any(u => u.Email == "khachhang@autogarage.vn"))
        {
            db.Users.Add(new User
            {
                Name = "Nguyễn Văn Khách",
                Email = "khachhang@autogarage.vn",
                Phone = "0901234567",
                PasswordHash = hasher.Hash("123456"),
                Role = "customer"
            });
        }
        db.SaveChanges();

        // 2. Seed Cars
        if (!db.Cars.Any())
        {
            db.Cars.AddRange(
                new Car
                {
                    Name = "Toyota Camry 2024",
                    Brand = "Toyota",
                    Price = "1.150.000.000 ₫",
                    ImageUrl = "🚗",
                    Year = 2024,
                    Engine = "2.5L Hybrid",
                    Fuel = "Xăng / Hybrid",
                    Seats = 5,
                    Transmission = "Tự động 8 cấp",
                    Origin = "Nhật Bản",
                    Description = "Sedan hạng D sang trọng, tiết kiệm nhiên liệu, an toàn 5 sao."
                },
                new Car
                {
                    Name = "Honda CR-V 2024",
                    Brand = "Honda",
                    Price = "1.050.000.000 ₫",
                    ImageUrl = "🚙",
                    Year = 2024,
                    Engine = "1.5L Turbo",
                    Fuel = "Xăng",
                    Seats = 7,
                    Transmission = "CVT",
                    Origin = "Nhật Bản",
                    Description = "SUV 7 chỗ đa dụng, phù hợp gia đình, khoang hành lý rộng."
                },
                new Car
                {
                    Name = "Ford Ranger Wildtrak",
                    Brand = "Ford",
                    Price = "920.000.000 ₫",
                    ImageUrl = "🛻",
                    Year = 2024,
                    Engine = "2.0L Bi-Turbo",
                    Fuel = "Dầu",
                    Seats = 5,
                    Transmission = "Tự động 10 cấp",
                    Origin = "Mỹ",
                    Description = "Bán tải mạnh mẽ, khả năng off-road tốt, nội thất tiện nghi."
                },
                new Car
                {
                    Name = "Mazda CX-5 2024",
                    Brand = "Mazda",
                    Price = "880.000.000 ₫",
                    ImageUrl = "🚙",
                    Year = 2024,
                    Engine = "2.0L SkyActiv",
                    Fuel = "Xăng",
                    Seats = 5,
                    Transmission = "Tự động 6 cấp",
                    Origin = "Nhật Bản",
                    Description = "Crossover thiết kế KODO đẹp mắt, vận hành êm ái."
                },
                new Car
                {
                    Name = "BMW 320i 2024",
                    Brand = "BMW",
                    Price = "1.550.000.000 ₫",
                    ImageUrl = "🏎️",
                    Year = 2024,
                    Engine = "2.0L TwinPower Turbo",
                    Fuel = "Xăng",
                    Seats = 5,
                    Transmission = "Tự động 8 cấp",
                    Origin = "Đức",
                    Description = "Sedan thể thao hạng sang, cảm giác lái phấn khích."
                },
                new Car
                {
                    Name = "Mercedes C200 2024",
                    Brand = "Mercedes-Benz",
                    Price = "1.690.000.000 ₫",
                    ImageUrl = "🚗",
                    Year = 2024,
                    Engine = "1.5L Turbo + EQ Boost",
                    Fuel = "Xăng",
                    Seats = 5,
                    Transmission = "Tự động 9 cấp",
                    Origin = "Đức",
                    Description = "Sedan sang trọng, công nghệ MBUX hiện đại."
                }
            );
            db.SaveChanges();
        }

        // 3. Seed Accessories
        if (!db.Accessories.Any())
        {
            db.Accessories.AddRange(
                new Accessory
                {
                    Name = "Ghế Da Cao Cấp",
                    Category = "Nội thất",
                    Price = "15.000.000 ₫",
                    ImageUrl = "🪑",
                    Description = "Ghế da bò thật, may thủ công, có sưởi và làm mát.",
                    Material = "Da bò thật",
                    Warranty = "24 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Mazda CX-5,BMW 320i,Mercedes C200"
                },
                new Accessory
                {
                    Name = "Màn hình Android 12\"",
                    Category = "Nội thất",
                    Price = "8.500.000 ₫",
                    ImageUrl = "📺",
                    Description = "Màn hình Android 12 inch, chip xử lý mạnh, tích hợp CarPlay/Android Auto.",
                    Material = "Kính cường lực",
                    Warranty = "12 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Ford Ranger,Mazda CX-5"
                },
                new Accessory
                {
                    Name = "Thảm Lót Sàn 5D",
                    Category = "Nội thất",
                    Price = "2.200.000 ₫",
                    ImageUrl = "🧶",
                    Description = "Thảm da lót sàn 5D chống thấm, vừa khít theo xe.",
                    Material = "Da tổng hợp",
                    Warranty = "6 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Mazda CX-5,Ford Ranger,Hyundai"
                },
                new Accessory
                {
                    Name = "Cánh Gió Thể Thao",
                    Category = "Ngoại thất",
                    Price = "5.500.000 ₫",
                    ImageUrl = "🪽",
                    Description = "Cánh gió carbon fiber, tăng tính khí động học và thẩm mỹ.",
                    Material = "Carbon Fiber",
                    Warranty = "12 tháng",
                    Compat = "Honda CR-V,Mazda CX-5,BMW 320i,Ford Ranger"
                },
                new Accessory
                {
                    Name = "Camera 360 Độ",
                    Category = "An toàn",
                    Price = "7.800.000 ₫",
                    ImageUrl = "📹",
                    Description = "Camera 360 độ, hiển thị góc nhìn toàn cảnh, hỗ trợ đỗ xe.",
                    Material = "Cảm biến CMOS",
                    Warranty = "18 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Ford Ranger,Mazda CX-5,BMW 320i,Mercedes C200"
                },
                new Accessory
                {
                    Name = "Lốp Michelin Pilot Sport",
                    Category = "Vận hành",
                    Price = "4.200.000 ₫",
                    ImageUrl = "🛞",
                    Description = "Lốp thể thao cao cấp, bám đường tốt, giảm ồn.",
                    Material = "Cao su tổng hợp",
                    Warranty = "36 tháng",
                    Compat = "Toyota Camry,BMW 320i,Mercedes C200"
                },
                new Accessory
                {
                    Name = "Ắc Quy Khô 12V",
                    Category = "Vận hành",
                    Price = "2.900.000 ₫",
                    ImageUrl = "🔋",
                    Description = "Ắc quy khô 12V 70Ah, tuổi thọ cao, khởi động mạnh.",
                    Material = "Chì canxi",
                    Warranty = "24 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Ford Ranger,Mazda CX-5,BMW 320i,Mercedes C200"
                },
                new Accessory
                {
                    Name = "Vô Lăng Bọc Da",
                    Category = "Nội thất",
                    Price = "1.800.000 ₫",
                    ImageUrl = "🔘",
                    Description = "Vô lăng bọc da may tay, chống trượt, cảm giác cầm chắc.",
                    Material = "Da PU cao cấp",
                    Warranty = "6 tháng",
                    Compat = "Toyota Camry,Honda CR-V,Mazda CX-5,Ford Ranger,Hyundai"
                }
            );
            db.SaveChanges();
        }

        // 4. Seed Notifications
        if (!db.Notifications.Any())
        {
            db.Notifications.AddRange(
                new Notification
                {
                    Title = "Bảo dưỡng định kỳ",
                    Content = "Ưu đãi 20% gói bảo dưỡng định kỳ cho khách hàng mới trong tháng này.",
                    Date = DateTime.UtcNow
                },
                new Notification
                {
                    Title = "Khuyến mãi lốp xe",
                    Content = "Mua 3 tặng 1 cho dòng lốp Michelin Pilot Sport từ ngày 1/11 đến 30/11.",
                    Date = DateTime.UtcNow.AddDays(-2)
                }
            );
            db.SaveChanges();
        }

        // 5. Seed Events
        if (!db.Events.Any())
        {
            db.Events.AddRange(
                new Event
                {
                    Name = "Flash Sale Phụ Kiện",
                    Code = "SALE30",
                    EndDate = DateTime.UtcNow.AddDays(7),
                    Status = "Đang diễn ra"
                },
                new Event
                {
                    Name = "Chào Bạn Mới",
                    Code = "WELCOME10",
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Status = "Đang diễn ra"
                },
                new Event
                {
                    Name = "Miễn Phí Vận Chuyển",
                    Code = "SHIPFREE",
                    EndDate = DateTime.UtcNow.AddDays(14),
                    Status = "Đang diễn ra"
                }
            );
            db.SaveChanges();
        }

        // 6. Seed Chat
        if (!db.ChatMessages.Any())
        {
            db.ChatMessages.AddRange(
                new ChatMessage
                {
                    From = "customer",
                    Sender = "Khách Hàng (Demo)",
                    SenderEmail = "demo@customer.vn",
                    Text = "Xin chào, tôi cần tư vấn về xe Toyota Camry!",
                    Time = DateTime.UtcNow.AddMinutes(-10)
                },
                new ChatMessage
                {
                    From = "admin",
                    Sender = "AutoGarage Admin",
                    SenderEmail = "admin@autogarage.vn",
                    Text = "Chào bạn, Toyota Camry hiện đang có sẵn xe giao ngay. Bạn cần tư vấn về giá hay thủ tục trả góp?",
                    Time = DateTime.UtcNow.AddMinutes(-5)
                }
            );
            db.SaveChanges();
        }
    }
}
