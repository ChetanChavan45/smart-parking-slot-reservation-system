using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IParkingMjestoService
    {
        Task<List<ParkingMjesto>> GetAllAsync();
        Task<List<ParkingMjesto>> SearchAsync(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost);
        Task<ParkingMjesto?> GetRecommendedAsync(IEnumerable<ParkingMjesto> parkingMjesta);
        Task<ParkingMjesto?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(ParkingMjesto parkingMjesto);
        Task UpdateAsync(ParkingMjesto parkingMjesto);
        Task DeleteAsync(int id);
    }
}
