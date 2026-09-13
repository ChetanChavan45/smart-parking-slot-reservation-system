using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class PodrskaRepository : IPodrskaRepository
    {
        private readonly ApplicationDbContext _context;

        public PodrskaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PodrskaZahtjev>> GetAllAsync(StatusPodrske? status = null)
        {
            var query = _context.PodrskaZahtjevi
                .AsNoTracking()
                .Include(z => z.Korisnik)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(z => z.Status == status.Value);

            return await query
                .OrderByDescending(z => z.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<List<PodrskaZahtjev>> GetByKorisnikIdAsync(
            string korisnikId,
            StatusPodrske? status = null)
        {
            var query = _context.PodrskaZahtjevi
                .AsNoTracking()
                .Include(z => z.Korisnik)
                .Where(z => z.KorisnikId == korisnikId);

            if (status.HasValue)
                query = query.Where(z => z.Status == status.Value);

            return await query
                .OrderByDescending(z => z.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<PodrskaZahtjev?> GetByIdAsync(int id)
        {
            return await _context.PodrskaZahtjevi
                .Include(z => z.Korisnik)
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task AddAsync(PodrskaZahtjev zahtjev)
        {
            await _context.PodrskaZahtjevi.AddAsync(zahtjev);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PodrskaZahtjev zahtjev)
        {
            _context.PodrskaZahtjevi.Update(zahtjev);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var zahtjev = await _context.PodrskaZahtjevi.FindAsync(id);

            if (zahtjev == null)
                return;

            _context.PodrskaZahtjevi.Remove(zahtjev);
            await _context.SaveChangesAsync();
        }
    }
}
