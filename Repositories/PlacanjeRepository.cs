using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class PlacanjeRepository : IPlacanjeRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacanjeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Placanje>> GetAllAsync()
        {
            return await _context.Placanja
                .Include(p => p.Rezervacija)
                .ToListAsync();
        }

        public async Task<Placanje?> GetByIdAsync(int id)
        {
            return await _context.Placanja
                .Include(p => p.Rezervacija)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Placanje placanje)
        {
            await _context.Placanja.AddAsync(placanje);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Placanje placanje)
        {
            _context.Placanja.Update(placanje);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var placanje = await _context.Placanja.FindAsync(id);

            if (placanje != null)
            {
                _context.Placanja.Remove(placanje);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PostojiPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return await _context.Placanja.AnyAsync(p =>
                p.RezervacijaId == rezervacijaId &&
                p.StatusPlacanja == PametniParkingSistem.Enums.StatusPlacanja.Uspjesno);
        }

        public async Task<Placanje?> GetUspjesnoPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return await _context.Placanja
                .FirstOrDefaultAsync(p =>
                    p.RezervacijaId == rezervacijaId &&
                    p.StatusPlacanja == PametniParkingSistem.Enums.StatusPlacanja.Uspjesno);
        }
    }
}