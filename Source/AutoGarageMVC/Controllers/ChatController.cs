namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;

public class ChatController : Controller
{
    private readonly AutoGarageDbContext _db;

    public ChatController(AutoGarageDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetMessages(string? sessionId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return StatusCode(401, new { message = "Vui lòng đăng nhập hoặc đăng ký tài khoản để sử dụng Trợ lý ảo AI." });
        }

        var userEmail = User.Identity.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        if (user == null)
        {
            return StatusCode(401, new { message = "Không tìm thấy thông tin tài khoản." });
        }

        var userSessionId = "user_" + user.Id;

        var messages = _db.ChatMessages
            .Where(m => m.SessionId == userSessionId || m.UserId == user.Id)
            .OrderBy(m => m.Time)
            .Select(m => new
            {
                m.Id,
                m.SessionId,
                m.From,
                m.Sender,
                m.Text,
                Time = m.Time.ToString("HH:mm")
            })
            .ToList();

        return Json(new { sessionId = userSessionId, messages });
    }

    [HttpPost]
    public IActionResult SendMessage([FromBody] ChatMessageRequest request)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return StatusCode(401, new { message = "Vui lòng đăng nhập hoặc đăng ký tài khoản để trò chuyện với Trợ lý ảo AI." });
        }

        if (string.IsNullOrWhiteSpace(request?.Text))
        {
            return BadRequest(new { message = "Nội dung tin nhắn không được để trống." });
        }

        var userEmail = User.Identity.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        if (user == null)
        {
            return StatusCode(401, new { message = "Không tìm thấy thông tin tài khoản." });
        }

        // Mỗi tài khoản là 1 hộp thoại riêng, session gắn cố định với UserId
        var sessionId = "user_" + user.Id;
        var senderName = user.Name;

        // 1. Lưu tin nhắn của khách hàng
        var userMsg = new ChatMessage
        {
            SessionId = sessionId,
            UserId = user.Id,
            From = "customer",
            Sender = senderName,
            SenderEmail = userEmail,
            Text = request.Text.Trim(),
            Time = DateTime.Now
        };
        _db.ChatMessages.Add(userMsg);
        _db.SaveChanges();

        // 2. Trợ lý ảo AI phân tích và trả lời tự động dựa trên kho dữ liệu của AutoGarage
        var replyText = GenerateAiReply(request.Text.Trim());

        var aiMsg = new ChatMessage
        {
            SessionId = sessionId,
            UserId = user.Id, // Gắn với cuộc hội thoại của tài khoản này
            From = "ai",
            Sender = "AutoGarage AI",
            SenderEmail = "ai@autogarage.vn",
            Text = replyText,
            Time = DateTime.Now
        };
        _db.ChatMessages.Add(aiMsg);
        _db.SaveChanges();

        return Json(new
        {
            success = true,
            sessionId = sessionId,
            userMessage = new
            {
                userMsg.Id,
                userMsg.From,
                userMsg.Sender,
                userMsg.Text,
                Time = userMsg.Time.ToString("HH:mm")
            },
            aiReply = new
            {
                aiMsg.Id,
                aiMsg.From,
                aiMsg.Sender,
                aiMsg.Text,
                Time = aiMsg.Time.ToString("HH:mm")
            }
        });
    }

    private string GetOrSetSessionId(string? clientSessionId)
    {
        if (!string.IsNullOrWhiteSpace(clientSessionId))
        {
            return clientSessionId.Trim();
        }

        var sessionCookie = Request.Cookies["AutoGarage_ChatSession"];
        if (!string.IsNullOrWhiteSpace(sessionCookie))
        {
            return sessionCookie;
        }

        var newSessionId = Guid.NewGuid().ToString("N");
        Response.Cookies.Append("AutoGarage_ChatSession", newSessionId, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(30),
            HttpOnly = false,
            SameSite = SameSiteMode.Lax
        });
        return newSessionId;
    }

    private string GenerateAiReply(string question)
    {
        var q = question.ToLowerInvariant();

        // Chào hỏi
        if (q.Contains("chào") || q.Contains("hello") || q.Contains("hi") || q.Contains("bắt đầu"))
        {
            return "Xin chào quý khách! Em là Trợ lý ảo AI của AutoGarage. Em có thể hỗ trợ tư vấn giá xe, các mẫu xe bán chạy, phụ kiện nâng cấp, bảo dưỡng xe định kỳ hoặc hotline hỗ trợ. Quý khách đang quan tâm đến xe hay phụ kiện nào ạ?";
        }

        // Hỏi địa chỉ, giờ mở cửa
        if (q.Contains("địa chỉ") || q.Contains("ở đâu") || q.Contains("showroom") || q.Contains("vị trí") || q.Contains("chi nhánh"))
        {
            return "Showroom trung tâm của AutoGarage tọa lạc tại: 123 Đường Số 1, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh. Giờ mở cửa: 08:00 - 20:00 tất cả các ngày trong tuần. Rất hân hạnh được đón tiếp quý khách!";
        }

        // Hỏi hotline / số điện thoại / liên hệ
        if (q.Contains("hotline") || q.Contains("số điện thoại") || q.Contains("sđt") || q.Contains("gọi") || q.Contains("liên hệ") || q.Contains("cứu hộ"))
        {
            return "Dạ số điện thoại hỗ trợ của AutoGarage:\n📞 Hotline bán hàng & tư vấn: 0901 234 567\n🚨 Cứu hộ khẩn cấp 24/7: 0999 999 999\nQuý khách có thể bấm nút gọi tròn màu đỏ ở góc màn hình để gọi ngay ạ!";
        }

        // Hỏi giá xe hoặc danh sách xe
        if (q.Contains("xe") || q.Contains("camry") || q.Contains("toyota") || q.Contains("crv") || q.Contains("cr-v") || q.Contains("honda") || q.Contains("ranger") || q.Contains("ford") || q.Contains("mazda") || q.Contains("bmw") || q.Contains("mercedes"))
        {
            try
            {
                var cars = _db.Cars.Take(5).ToList();
                if (cars.Any())
                {
                    var matching = cars.Where(c => q.Contains(c.Brand.ToLower()) || q.Contains(c.Name.ToLower())).ToList();
                    var targetCars = matching.Any() ? matching : cars;

                    var info = string.Join("\n", targetCars.Select(c => $"• {c.Name} ({c.Brand}) - Giá: {c.Price} - Năm: {c.Year}"));
                    return $"Dạ hiện tại AutoGarage đang có các mẫu xe nổi bật:\n{info}\n\nQuý khách có thể bấm vào mục 'Các Hãng Xe' trên thanh menu để xem đầy đủ thông số và đặt mua nhé!";
                }
            }
            catch { }
            return "Dạ AutoGarage hiện đang phân phối chính hãng các dòng xe từ Toyota, Honda, Ford, Mazda, BMW, Mercedes với chính sách bảo hành chính hãng và hỗ trợ trả góp lãi suất ưu đãi. Quý khách có thể xem chi tiết tại mục 'Các Hãng Xe' trên thanh điều hướng ạ!";
        }

        // Hỏi phụ kiện
        if (q.Contains("phụ kiện") || q.Contains("màn hình") || q.Contains("thảm") || q.Contains("camera") || q.Contains("lốp") || q.Contains("ắc quy") || q.Contains("đồ chơi"))
        {
            try
            {
                var accs = _db.Accessories.Take(4).ToList();
                if (accs.Any())
                {
                    var info = string.Join("\n", accs.Select(a => $"• {a.Name} ({a.Category}) - Giá: {a.Price}"));
                    return $"Dạ AutoGarage cung cấp 100% phụ kiện chính hãng, bảo hành lên tới 36 tháng:\n{info}\n\nQuý khách có thể vào mục 'Phụ Kiện' để thêm vào giỏ hàng và nhận ưu đãi miễn phí giao hàng toàn quốc ạ!";
                }
            }
            catch { }
            return "Dạ bên em có đầy đủ các danh mục phụ kiện: Nội thất, Ngoại thất, An toàn và Vận hành. Tất cả đều là hàng chính hãng có giấy tờ và bảo hành rõ ràng ạ!";
        }

        // Hỏi bảo dưỡng, sửa chữa, cứu hộ
        if (q.Contains("bảo dưỡng") || q.Contains("sửa") || q.Contains("sơn") || q.Contains("detailing") || q.Contains("thay dầu") || q.Contains("nhớt"))
        {
            return "Xưởng dịch vụ AutoGarage cung cấp các dịch vụ tiêu chuẩn 5 sao:\n1. Bảo dưỡng định kỳ 30 hạng mục.\n2. Đồng sơn phục hồi chuẩn quốc tế.\n3. Chăm sóc Detailing & Phủ Ceramic cao cấp.\n4. Cứu hộ lưu động 24/7.\nQuý khách vui lòng gọi 0901 234 567 để đặt lịch hẹn không phải chờ đợi nhé!";
        }

        // Hỏi khuyến mãi, flash sale, coupon
        if (q.Contains("khuyến mãi") || q.Contains("sale") || q.Contains("mã") || q.Contains("giảm giá") || q.Contains("coupon") || q.Contains("ưu đãi"))
        {
            return "🎉 AutoGarage đang có các ưu đãi cực hot:\n• Mã SALE30: Giảm 30% cho phụ kiện\n• Mã WELCOME10: Giảm 10% đơn hàng đầu tiên\n• Mã SHIPFREE: Miễn phí vận chuyển toàn quốc cho đơn từ 500.000 ₫!\nQuý khách nhanh tay nhập mã ở bước thanh toán nhé!";
        }

        // Mặc định
        return "Cảm ơn quý khách đã gửi câu hỏi! AutoGarage luôn sẵn sàng hỗ trợ. Quý khách có thể hỏi em về các mẫu xe hơi, phụ kiện chính hãng, báo giá, khuyến mãi hoặc dịch vụ bảo dưỡng sửa chữa. Nếu cần tư vấn trực tiếp ngay, xin gọi hotline 0901 234 567 ạ!";
    }

    public class ChatMessageRequest
    {
        public string Text { get; set; } = "";
        public string? SessionId { get; set; }
        public string? Sender { get; set; }
    }
}
