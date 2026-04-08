using Bungalov.Core.Varliklar;
using System.Linq.Expressions;

namespace Bungalov.Business.Interfaces;

public interface IAmenityService
{
    Task<List<Amenity>> GetAllAmenitiesAsync();
    Task<Amenity?> GetAmenityByIdAsync(int id);
    Task AddAmenityAsync(Amenity amenity);
    Task UpdateAmenityAsync(Amenity amenity);
    Task DeleteAmenityAsync(int id);
}
