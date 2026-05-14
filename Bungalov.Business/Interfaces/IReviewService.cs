using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Interfaces;

public interface IReviewService
{
    Task AddReviewAsync(Review review);
    Task<List<Review>> GetReviewsByBungalowIdAsync(int bungalowId);
    Task<List<Review>> GetAllReviewsWithDetailsAsync();
    Task<Review?> GetReviewByReservationIdAsync(int reservationId);
    Task<List<Reservation>> GetPendingReviewsByUserIdAsync(string userId);
}
