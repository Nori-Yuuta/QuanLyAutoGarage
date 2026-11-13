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
        if (!file.ContentType.StartsWith("image/")) return null;
        var folder = Path.Combine(_env.WebRootPath, subFolder);
        Directory.CreateDirectory(folder);
        var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(folder, name);
        using var fs = File.Create(path);
        await file.CopyToAsync(fs);
        return $"/{subFolder}/{name}";
    }
}
