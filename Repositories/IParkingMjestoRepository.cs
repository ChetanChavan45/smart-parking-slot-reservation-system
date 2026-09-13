using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IParkingMjestoRepository
    {
        Task<List<ParkingMjesto>> GetAllAsync();
        Task<List<ParkingMjesto>> SearchAsync(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost);
        Task<ParkingMjesto?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(ParkingMjesto parkingMjesto);
        Task UpdateAsync(ParkingMjesto parkingMjesto);
        Task DeleteAsync(int id);
    }
}
