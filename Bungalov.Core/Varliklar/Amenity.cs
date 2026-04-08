using System.Collections.Generic;

namespace Bungalov.Core.Varliklar;

public class Amenity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string IconCode { get; set; } = "bi-star"; // Default Bootstrap icon

    public ICollection<Bungalow> Bungalows { get; set; } = new List<Bungalow>();
}
