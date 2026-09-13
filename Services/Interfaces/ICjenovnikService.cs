using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface ICjenovnikService
    {
        Task<List<Cjenovnik>> GetAllAsync();
        Task<Cjenovnik?> GetByIdAsync(int id);
        Task AddAsync(Cjenovnik cjenovnik);
        Task UpdateAsync(Cjenovnik cjenovnik);
        Task DeleteAsync(int id);
    }
}