using System.ComponentModel.DataAnnotations;

namespace Bungalov.Core.Varliklar;

public class Guest : BaseEntity
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(11, MinimumLength = 11)]
    public string IdentityNumber { get; set; } = string.Empty;

    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
}
