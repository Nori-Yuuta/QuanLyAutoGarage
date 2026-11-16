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
            var adminUser = db.Users.FirstOrDefault(u => u.Role == "admin");
            var custUser = db.Users.FirstOrDefault(u => u.Role == "customer");
            db.ChatMessages.AddRange(
                new ChatMessage
                {
                    UserId = custUser?.Id,
                    From = "customer",
                    Sender = "Khách Hàng (Demo)",
                    SenderEmail = "khachhang@autogarage.vn",
                    Text = "Xin chào, tôi cần tư vấn về xe Toyota Camry!",
                    Time = DateTime.UtcNow.AddMinutes(-10)
                },
                new ChatMessage
                {
                    UserId = adminUser?.Id,
                    From = "admin",
                    Sender = "AutoGarage Admin",
                    SenderEmail = "admin@autogarage.vn",
                    Text = "Chào bạn, Toyota Camry hiện đang có sẵn xe giao ngay. Bạn cần tư vấn về giá hay thủ tục trả góp?",
                    Time = DateTime.UtcNow.AddMinutes(-5)
                }
            );
            db.SaveChanges();
        }

        // 7. Seed News
        if (!db.News.Any())
        {
            var adminUser = db.Users.FirstOrDefault(u => u.Role == "admin");
            db.News.AddRange(
                new News
                {
                    Title = "Đánh Giá Toyota Camry 2024: Đẳng Cấp Sedan Hạng D Trở Lại",
                    Category = "Xe Mới",
                    Summary = "Toyota Camry 2024 mang diện mạo hoàn toàn mới với thiết kế thể thao, hệ truyền động Hybrid tiết kiệm nhiên liệu vượt trội và gói an toàn TSS 3.0.",
                    Content = @"<p>Toyota Camry 2024 thế hệ mới chính thức ra mắt với nhiều cải tiến đột phá từ ngoại thất, nội thất đến hệ truyền động. Chiếc sedan hạng D huyền thoại này tiếp tục khẳng định vị thế dẫn đầu phân khúc tại thị trường Việt Nam.</p>
<h4>1. Thiết Kế Ngoại Thất Hiện Đại</h4>
<p>Lấy cảm hứng từ ngôn ngữ thiết kế Hammerhead mới của Toyota, Camry 2024 sở hữu cụm đèn định vị ban ngày hình chữ C sắc sảo, lưới tản nhiệt mở rộng và đường gân dập nổi thể thao vuốt dài dọc thân xe. Mâm xe 19 inch đa chấu tạo điểm nhấn khỏe khoắn và sang trọng.</p>
<h4>2. Nội Thất Tiện Nghi Và Công Nghệ Vượt Trội</h4>
<p>Khoang lái được tái thiết kế hoàn toàn với cụm đồng hồ kỹ thuật số 12.3 inch, màn hình giải trí trung tâm 12.3 inch hỗ trợ kết nối Apple CarPlay và Android Auto không dây. Hệ thống âm thanh vòm JBL 9 loa mang lại trải nghiệm âm thanh sống động.</p>
<h4>3. Động Cơ Hybrid Tiết Kiệm Nhiên Liệu</h4>
<p>Phiên bản Hybrid thế hệ thứ 5 kết hợp động cơ 2.5L 4 xi-lanh cùng mô-tơ điện sản sinh công suất tổng hợp lên đến 225 mã lực, mức tiêu thụ nhiên liệu chỉ khoảng 4.5L/100km trên đường hỗn hợp.</p>
<h4>4. Gói An Toàn Chủ Động Toyota Safety Sense 3.0</h4>
<p>Hệ thống cảnh báo tiền va chạm, kiểm soát hành trình chủ động radar ở mọi dải tốc độ, hỗ trợ giữ làn đường và cảnh báo điểm mù mang lại sự an tâm tuyệt đối cho người lái.</p>",
                    ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=800&q=80",
                    AuthorId = adminUser?.Id,
                    Views = 152,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new News
                {
                    Title = "5 Phụ Kiện Ô Tô Không Thể Thiếu Khi Mua Xe Mới",
                    Category = "Kinh Nghiệm",
                    Summary = "Từ camera 360 độ, màn hình Android đến thảm lót sàn 5D — điểm danh những trang bị giúp xế cưng tiện nghi, sang trọng và an toàn tối đa.",
                    Content = @"<p>Sau khi nhận chiếc xe mới tinh từ showroom, việc trang bị thêm những phụ kiện cần thiết không chỉ giúp xe thêm tiện nghi mà còn bảo vệ nội ngoại thất và nâng cao độ an toàn cho cả gia đình.</p>
<h4>1. Camera Hành Trình & Camera 360 Độ</h4>
<p>Đây là trang bị quan trọng hàng đầu giúp ghi lại toàn bộ hành trình, cung cấp bằng chứng khách quan khi xảy ra va chạm và hỗ trợ đỗ xe dễ dàng trong các không gian hẹp.</p>
<h4>2. Phim Cách Nhiệt Cao Cấp</h4>
<p>Với khí hậu nhiệt đới nắng nóng tại Việt Nam, dán phim cách nhiệt giúp cản tia cực tím UV đến 99%, giảm nhiệt độ cabin xe, bảo vệ da và các chi tiết nội thất nhựa, da khỏi lão hóa.</p>
<h4>3. Thảm Lót Sàn Ô Tô Tràn Viền</h4>
<p>Thảm lót sàn giữ cho sàn nỉ nguyên bản luôn sạch sẽ, ngăn nước, bụi bẩn và dễ dàng tháo rời vệ sinh nhanh chóng.</p>
<h4>4. Cảm Biến Áp Suất Lốp (TPMS)</h4>
<p>Theo dõi liên tục áp suất và nhiệt độ của từng bánh xe, cảnh báo sớm nguy cơ xì lốp hoặc nổ lốp khi di chuyển trên cao tốc.</p>
<h4>5. Bơm Lốp Điện Tử Cầm Tay & Kích Bình</h4>
<p>Bộ cứu hộ mini giúp bạn tự tin xử lý sự cố bất ngờ trên đường xa mà không cần phụ thuộc vào xe cứu hộ.</p>",
                    ImageUrl = "https://images.unsplash.com/photo-1486006920555-c77dce18193b?auto=format&fit=crop&w=800&q=80",
                    AuthorId = adminUser?.Id,
                    Views = 240,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new News
                {
                    Title = "Xu Hướng Xe Hybrid Và Xe Điện Tại Thị Trường Việt Nam 2024",
                    Category = "Công Nghệ",
                    Summary = "Hạ tầng trạm sạc cùng chính sách ưu đãi thuế đang mở ra kỷ nguyên mới cho các dòng xe điện và Hybrid bảo vệ môi trường tại Việt Nam.",
                    Content = @"<p>Năm 2024 đánh dấu bước chuyển mình mạnh mẽ của thị trường ô tô Việt Nam khi các dòng xe điện (EV) và xe lai Hybrid (HEV/PHEV) ngày càng được người tiêu dùng đón nhận nồng nhiệt.</p>
<h4>1. Ưu Thế Vượt Trội Về Chi Phí Vận Hành</h4>
<p>Giá xăng dầu biến động cùng công nghệ pin thế hệ mới giúp chi phí vận hành mỗi km của xe điện và Hybrid thấp hơn từ 40% đến 60% so với xe thuần động cơ đốt trong truyền thống.</p>
<h4>2. Chính Sách Khuyến Khích Từ Chính Phủ</h4>
<p>Chính sách miễn lệ phí trước bạ cho ô tô điện cùng các ưu đãi thuế tiêu thụ đặc biệt đã tạo cú hích lớn giúp người tiêu dùng dễ dàng tiếp cận công nghệ xanh.</p>
<h4>3. Hạ Tầng Trạm Sạc Mở Rộng Thần Tốc</h4>
<p>Mạng lưới trạm sạc công cộng phủ khắp các trạm dừng nghỉ cao tốc, trung tâm thương mại và tòa nhà văn phòng giải tỏa nỗi lo hết pin trên các chặng đường dài.</p>",
                    ImageUrl = "https://images.unsplash.com/photo-1593941707882-a5bba14938c7?auto=format&fit=crop&w=800&q=80",
                    AuthorId = adminUser?.Id,
                    Views = 310,
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                }
            );
            db.SaveChanges();
        }
    }
}
