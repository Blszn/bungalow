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

    public async Task<bool> IsAvailableAsync(int bungalowId, DateTime checkIn, DateTime checkOut)
    {
        // CheckIn >= ExistingCheckOut OR CheckOut <= ExistingCheckIn ise çakışma yoktur.
        // Tersi durumda (CheckIn < ExistingCheckOut AND CheckOut > ExistingCheckIn) çakışma vardır.
        var reservations = await _unitOfWork.GetRepository<Reservation>().GetByFilterAsync(r => 
            r.BungalowId == bungalowId && 
            checkIn < r.CheckOutDate && 
            checkOut > r.CheckInDate);
            
        return !reservations.Any();
    }

    public async Task<List<DateTime>> GetBookedDatesAsync(int bungalowId)
    {
        var reservations = await _unitOfWork.GetRepository<Reservation>().GetByFilterAsync(r => r.BungalowId == bungalowId);
        var dates = new List<DateTime>();

        foreach (var res in reservations)
        {
            var date = res.CheckInDate.Date;
            while (date < res.CheckOutDate.Date)
            {
                dates.Add(date);
                date = date.AddDays(1);
            }
        }
        return dates.Distinct().ToList();
    }
}
