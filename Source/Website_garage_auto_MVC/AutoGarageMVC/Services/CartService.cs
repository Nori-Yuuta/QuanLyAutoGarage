namespace AutoGarageMVC.Services;

using System.Text.Json;
using Microsoft.AspNetCore.Http;

public record CartItem(int Id, string Name, decimal Price, string? ImageUrl, string? Category, int Qty);

public class CartService
{
    private const string Key = "cart";
    private readonly ISession? _session;

    public CartService(IHttpContextAccessor h) => _session = h.HttpContext?.Session;

    public List<CartItem> Get()
    {
        if (_session == null) return new();
        var json = _session.GetString(Key);
        return string.IsNullOrEmpty(json) ? new() : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
    }

    private void Save(List<CartItem> items) =>
        _session?.SetString(Key, JsonSerializer.Serialize(items));

    public void Add(CartItem item)
    {
        var list = Get();
        var idx = list.FindIndex(i => i.Id == item.Id);
        if (idx >= 0) list[idx] = list[idx] with { Qty = list[idx].Qty + item.Qty };
        else list.Add(item);
        Save(list);
    }

    public void UpdateQty(int id, int qty)
    {
        var list = Get();
        var i = list.FindIndex(x => x.Id == id);
        if (i < 0) return;
        list[i] = list[i] with { Qty = Math.Max(1, qty) };
        Save(list);
    }

    public void Remove(int id)
    {
        var l = Get();
        l.RemoveAll(x => x.Id == id);
        Save(l);
    }

    public void Clear() => Save(new());

    public int Count => Get().Sum(i => i.Qty);
    public decimal Subtotal => Get().Sum(i => i.Price * i.Qty);
    public decimal Shipping => Subtotal >= 500_000 || Subtotal == 0 ? 0 : 30_000;
}
