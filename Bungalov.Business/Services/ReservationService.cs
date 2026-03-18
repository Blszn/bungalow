using Bungalov.Business.Interfaces;
using Bungalov.Core.Interfaces;
using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        await _unitOfWork.GetRepository<Reservation>().AddAsync(reservation);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteReservationAsync(int id)
    {
        var reservation = await _unitOfWork.GetRepository<Reservation>().GetByIdAsync(id);
        if (reservation != null)
        {
            _unitOfWork.GetRepository<Reservation>().Delete(reservation);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await _unitOfWork.GetRepository<Reservation>().GetAllAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int id)
    {
        return await _unitOfWork.GetRepository<Reservation>().GetByIdAsync(id);
    }

    public async Task UpdateReservationAsync(Reservation reservation)
    {
        _unitOfWork.GetRepository<Reservation>().Update(reservation);
        await _unitOfWork.SaveAsync();
    }
}
