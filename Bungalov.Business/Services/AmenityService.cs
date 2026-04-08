using Bungalov.Business.Interfaces;
using Bungalov.Core.Interfaces;
using Bungalov.Core.Varliklar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bungalov.Business.Services;

public class AmenityService : IAmenityService
{
    private readonly IUnitOfWork _unitOfWork;

    public AmenityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddAmenityAsync(Amenity amenity)
    {
        await _unitOfWork.GetRepository<Amenity>().AddAsync(amenity);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteAmenityAsync(int id)
    {
        var amenity = await _unitOfWork.GetRepository<Amenity>().GetByIdAsync(id);
        if (amenity != null)
        {
            _unitOfWork.GetRepository<Amenity>().Delete(amenity);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<List<Amenity>> GetAllAmenitiesAsync()
    {
        return await _unitOfWork.GetRepository<Amenity>().GetAllAsync();
    }

    public async Task<Amenity?> GetAmenityByIdAsync(int id)
    {
        var result = await _unitOfWork.GetRepository<Amenity>().GetByFilterAsync(a => a.Id == id);
        return result.FirstOrDefault();
    }

    public async Task UpdateAmenityAsync(Amenity amenity)
    {
        _unitOfWork.GetRepository<Amenity>().Update(amenity);
        await _unitOfWork.SaveAsync();
    }
}
