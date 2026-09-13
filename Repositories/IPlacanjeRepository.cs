using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IPlacanjeRepository
    {
        Task<List<Placanje>> GetAllAsync();
        Task<Placanje?> GetByIdAsync(int id);
        Task AddAsync(Placanje placanje);
        Task UpdateAsync(Placanje placanje);
        Task DeleteAsync(int id);
        Task<bool> PostojiPlacanjeZaRezervacijuAsync(int rezervacijaId);
        Task<Placanje?> GetUspjesnoPlacanjeZaRezervacijuAsync(int rezervacijaId);
    }
}