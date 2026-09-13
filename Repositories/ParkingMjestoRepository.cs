using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class ParkingMjestoRepository : IParkingMjestoRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingMjestoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<ParkingMjesto>> GetAllAsync()
        {
            return _context.ParkingMjesta
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<ParkingMjesto>> SearchAsync(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost)
        {
            IQueryable<ParkingMjesto> query = _context.ParkingMjesta
                .AsNoTracking();

            if (zonaId.HasValue)
                query = query.Where(p => p.ParkingZonaId == zonaId.Value);

            if (tipMjesta.HasValue)
                query = query.Where(p => p.TipMjesta == tipMjesta.Value);

            if (natkriveno.HasValue)
                query = query.Where(p => p.Natkriveno == natkriveno.Value);

            if (minCijena.HasValue)
                query = query.Where(p => p.CijenaPoSatu >= minCijena.Value);

            if (maxCijena.HasValue)
                query = query.Where(p => p.CijenaPoSatu <= maxCijena.Value);

            if (maxUdaljenost.HasValue)
                query = query.Where(p => p.UdaljenostOdUlaza <= maxUdaljenost.Value);

            return query.ToListAsync();
        }

        public Task<ParkingMjesto?> GetByIdAsync(int id)
        {
            return _context.ParkingMjesta
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.ParkingMjesta.AnyAsync(p => p.Id == id);
        }

        public async Task AddAsync(ParkingMjesto parkingMjesto)
        {
            await _context.ParkingMjesta.AddAsync(parkingMjesto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParkingMjesto parkingMjesto)
        {
            _context.ParkingMjesta.Update(parkingMjesto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var parkingMjesto = await _context.ParkingMjesta.FindAsync(id);

            if (parkingMjesto == null)
                return;

            _context.ParkingMjesta.Remove(parkingMjesto);
            await _context.SaveChangesAsync();
        }
    }
}
