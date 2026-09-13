using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class ParkingZonaService : IParkingZonaService
    {
        private readonly IParkingZonaRepository _repository;

        public ParkingZonaService(IParkingZonaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ParkingZona>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ParkingZona?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(ParkingZona parkingZona)
        {
            await _repository.AddAsync(parkingZona);
        }

        public async Task UpdateAsync(ParkingZona parkingZona)
        {
            await _repository.UpdateAsync(parkingZona);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}