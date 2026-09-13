using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IPodrskaRepository
    {
        Task<List<PodrskaZahtjev>> GetAllAsync(StatusPodrske? status = null);
        Task<List<PodrskaZahtjev>> GetByKorisnikIdAsync(string korisnikId, StatusPodrske? status = null);
        Task<PodrskaZahtjev?> GetByIdAsync(int id);
        Task AddAsync(PodrskaZahtjev zahtjev);
        Task UpdateAsync(PodrskaZahtjev zahtjev);
        Task DeleteAsync(int id);
    }
}
