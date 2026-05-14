using Bungalov.Business.Interfaces;
using Bungalov.Core.Interfaces;
using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddReviewAsync(Review review)
    {
        await _unitOfWork.GetRepository<Review>().AddAsync(review);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<Review>> GetReviewsByBungalowIdAsync(int bungalowId)
    {
        return await _unitOfWork.GetRepository<Review>().GetByFilterAsync(r => r.BungalowId == bungalowId, r => r.AppUser);
    }

    public async Task<List<Review>> GetAllReviewsWithDetailsAsync()
    {
        return await _unitOfWork.GetRepository<Review>().GetAllAsync(r => r.Bungalow, r => r.AppUser);
    }

    public async Task<Review?> GetReviewByReservationIdAsync(int reservationId)
    {
        var reviews = await _unitOfWork.GetRepository<Review>().GetByFilterAsync(r => r.ReservationId == reservationId);
        return reviews.FirstOrDefault();
    }

    public async Task<List<Reservation>> GetPendingReviewsByUserIdAsync(string userId)
    {
        // Get all completed and paid reservations for the user
        var reservations = await _unitOfWork.GetRepository<Reservation>().GetByFilterAsync(
            r => r.AppUserId == userId && r.CheckOutDate < DateTime.UtcNow && r.IsPaid,
            r => r.Bungalow);

        // Get all review IDs for this user
        var reviews = await _unitOfWork.GetRepository<Review>().GetByFilterAsync(r => r.AppUserId == userId);
        var reviewedReservationIds = reviews.Select(r => r.ReservationId).ToList();

        // Filter out reservations that are already reviewed
        return reservations.Where(r => !reviewedReservationIds.Contains(r.Id)).ToList();
    }
}
