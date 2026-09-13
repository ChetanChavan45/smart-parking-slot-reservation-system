using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IKorisnikRepository
    {
        Task<List<Korisnik>> GetAllAsync();
        Task<Korisnik?> GetByIdAsync(string id);
        Task AddAsync(Korisnik korisnik);
        Task UpdateAsync(Korisnik korisnik);
        Task DeleteAsync(string id);
    }
}