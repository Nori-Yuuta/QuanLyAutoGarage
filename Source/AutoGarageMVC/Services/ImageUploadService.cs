namespace AutoGarageMVC.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

public class ImageUploadService
{
    private readonly IWebHostEnvironment _env;
    public ImageUploadService(IWebHostEnvironment env) => _env = env;

    public async Task<string?> SaveAsync(IFormFile? file, string subFolder = "uploads")
    {
        if (file == null || file.Length == 0) return null;
        
        // Kiểm tra định dạng hợp lệ
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".jfif", ".bmp" };
        if (!allowedExts.Contains(ext))
        {
            // Nếu không phải phần mở rộng ảnh thông thường thì bỏ qua
            return null;
        }

        var folder = Path.Combine(_env.WebRootPath, subFolder);
        Directory.CreateDirectory(folder);
        var name = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(folder, name);
        using var fs = File.Create(path);
        await file.CopyToAsync(fs);
        return $"/{subFolder}/{name}";
    }
}
