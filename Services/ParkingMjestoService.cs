using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class ParkingMjestoService : IParkingMjestoService
    {
        private readonly IParkingMjestoRepository _repository;

        public ParkingMjestoService(IParkingMjestoRepository repository)
        {
            _repository = repository;
        }

        public Task<List<ParkingMjesto>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<List<ParkingMjesto>> SearchAsync(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost)
        {
            return _repository.SearchAsync(
                zonaId,
                tipMjesta,
                natkriveno,
                minCijena,
                maxCijena,
                maxUdaljenost);
        }

        public Task<ParkingMjesto?> GetRecommendedAsync(
            IEnumerable<ParkingMjesto> parkingMjesta)
        {
            var preporucenoMjesto = parkingMjesta
                .Where(p => p.Status != StatusMjesta.VanFunkcije)
                .OrderBy(p => p.UdaljenostOdUlaza)
                .ThenBy(p => p.CijenaPoSatu)
                .FirstOrDefault();

            return Task.FromResult(preporucenoMjesto);
        }

        public Task<ParkingMjesto?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _repository.ExistsAsync(id);
        }

        public Task AddAsync(ParkingMjesto parkingMjesto)
        {
            return _repository.AddAsync(parkingMjesto);
        }

        public Task UpdateAsync(ParkingMjesto parkingMjesto)
        {
            return _repository.UpdateAsync(parkingMjesto);
        }

        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
