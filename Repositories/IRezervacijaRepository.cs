using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IRezervacijaRepository
    {
        Task<List<Rezervacija>> GetAllAsync();
        Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId);
        Task<Rezervacija?> GetByIdAsync(int id);
        Task AddAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
        Task DeleteAsync(int id);
        Task<bool> PostojiPreklapanjeTerminaAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj);
        Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync();
    }
}