using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IParkingZonaService
    {
        Task<List<ParkingZona>> GetAllAsync();
        Task<ParkingZona?> GetByIdAsync(int id);
        Task AddAsync(ParkingZona parkingZona);
        Task UpdateAsync(ParkingZona parkingZona);
        Task DeleteAsync(int id);
    }
}