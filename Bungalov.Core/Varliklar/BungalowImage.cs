using System;

namespace Bungalov.Core.Varliklar;

public class BungalowImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public int BungalowId { get; set; }
    public Bungalow Bungalow { get; set; } = null!;
}
