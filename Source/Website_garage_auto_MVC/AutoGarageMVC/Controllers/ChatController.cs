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
    public IActionResult GetMessages()
    {
        var messages = _db.ChatMessages
            .OrderBy(m => m.Time)
            .Select(m => new
            {
                m.Id,
                m.From,
                m.Sender,
                m.SenderEmail,
                m.Text,
                Time = m.Time.ToString("HH:mm dd/MM/yyyy")
            })
            .ToList();

        return Json(messages);
    }

    [HttpPost]
    public IActionResult SendMessage([FromBody] ChatMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Text))
        {
            return BadRequest(new { message = "Nội dung tin nhắn không được để trống." });
        }

        var isUserAdmin = User.IsInRole("admin");
        var userEmail = User.Identity?.Name;
        var displayName = User.FindFirst("DisplayName")?.Value ?? (isUserAdmin ? "AutoGarage Admin" : "Khách Hàng");

        var msg = new ChatMessage
        {
            From = isUserAdmin ? "admin" : "customer",
            Sender = displayName,
            SenderEmail = userEmail,
            Text = request.Text.Trim(),
            Time = DateTime.UtcNow
        };

        _db.ChatMessages.Add(msg);
        _db.SaveChanges();

        return Json(new
        {
            success = true,
            message = new
            {
                msg.Id,
                msg.From,
                msg.Sender,
                msg.SenderEmail,
                msg.Text,
                Time = msg.Time.ToString("HH:mm dd/MM/yyyy")
            }
        });
    }

    public class ChatMessageRequest
    {
        public string Text { get; set; } = "";
    }
}
