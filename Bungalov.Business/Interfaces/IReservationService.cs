using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Interfaces;

public interface IReservationService
{
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int id);
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(int id);
    Task<bool> IsAvailableAsync(int bungalowId, DateTime checkIn, DateTime checkOut);
    Task<List<DateTime>> GetBookedDatesAsync(int bungalowId);
    Task<List<Reservation>> GetReservationsByUserIdAsync(string userId);
    Task<List<Reservation>> GetReservationsWithDetailsAsync();
}
