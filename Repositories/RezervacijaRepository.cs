using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;
using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Repositories
{
    public class RezervacijaRepository : IRezervacijaRepository
    {
        private readonly ApplicationDbContext _context;

        public RezervacijaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rezervacija>> GetAllAsync()
        {
            return await _context.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.ParkingMjesto)
                .Include(r => r.Recenzija)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId)
        {
            return await _context.Rezervacije
                .Include(r => r.ParkingMjesto)
                .Include(r => r.Recenzija)
                .Where(r => r.KorisnikId == korisnikId)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _context.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.ParkingMjesto)
                .Include(r => r.Recenzija)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Rezervacija rezervacija)
        {
            await _context.Rezervacije.AddAsync(rezervacija);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rezervacija rezervacija)
        {
            _context.Rezervacije.Update(rezervacija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rezervacija = await _context.Rezervacije.FindAsync(id);

            if (rezervacija != null)
            {
                _context.Rezervacije.Remove(rezervacija);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PostojiPreklapanjeTerminaAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj)
        {
            return await _context.Rezervacije.AnyAsync(r =>
                r.ParkingMjestoId == parkingMjestoId &&
                r.StatusRezervacije != StatusRezervacije.Otkazana &&
                pocetak < r.VrijemeKraja &&
                kraj > r.VrijemePocetka);
        }

        public async Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync()
        {
            return await _context.Rezervacije
                .Include(r => r.ParkingMjesto)
                .Where(r =>
                    r.VrijemeKraja < DateTime.Now &&
                    r.StatusRezervacije != StatusRezervacije.Otkazana &&
                    r.StatusRezervacije != StatusRezervacije.Zavrsena)
                .ToListAsync();
        }
    }
}