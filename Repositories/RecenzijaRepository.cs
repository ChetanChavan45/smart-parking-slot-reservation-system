using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class RecenzijaRepository : IRecenzijaRepository
    {
        private readonly ApplicationDbContext _context;

        public RecenzijaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recenzija>> GetAllAsync()
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .Include(r => r.Rezervacija)
                    .ThenInclude(rez => rez!.ParkingMjesto)
                .Where(r => !r.Obrisan)
                .OrderByDescending(r => r.Datum)
                .ToListAsync();
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .Include(r => r.Rezervacija)
                    .ThenInclude(rez => rez!.ParkingMjesto)
                .FirstOrDefaultAsync(r => r.Id == id && !r.Obrisan);
        }

        public async Task<Recenzija?> GetByRezervacijaIdAsync(int rezervacijaId)
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .Include(r => r.Rezervacija)
                .FirstOrDefaultAsync(r => r.RezervacijaId == rezervacijaId && !r.Obrisan);
        }

        public async Task<bool> ExistsForRezervacijaAsync(int rezervacijaId)
        {
            return await _context.Recenzije
                .AnyAsync(r => r.RezervacijaId == rezervacijaId && !r.Obrisan);
        }

        public async Task AddAsync(Recenzija recenzija)
        {
            _context.Recenzije.Add(recenzija);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Recenzija recenzija)
        {
            _context.Recenzije.Update(recenzija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recenzija = await _context.Recenzije.FindAsync(id);

            if (recenzija != null)
            {
                recenzija.Obrisan = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}