using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Interfaces;

public interface IReservationService
{
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int id);
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(int id);
}
