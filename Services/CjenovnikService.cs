using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class CjenovnikService : ICjenovnikService
    {
        private readonly ICjenovnikRepository _repository;

        public CjenovnikService(ICjenovnikRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Cjenovnik>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Cjenovnik?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Cjenovnik cjenovnik)
        {
            await _repository.AddAsync(cjenovnik);
        }

        public async Task UpdateAsync(Cjenovnik cjenovnik)
        {
            await _repository.UpdateAsync(cjenovnik);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}