using System;

namespace Bungalov.Core.Varliklar;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool Status { get; set; } = true;
}