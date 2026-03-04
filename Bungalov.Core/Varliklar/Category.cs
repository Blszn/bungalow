using System.Collections.Generic;

namespace Bungalov.Core.Varliklar;

public class Category : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<Bungalow> Bungalows { get; set; } = new List<Bungalow>();
}