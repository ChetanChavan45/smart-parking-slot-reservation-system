using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class ParkingZonaRepository : IParkingZonaRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingZonaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingZona>> GetAllAsync()
        {
            return await _context.ParkingZone.ToListAsync();
        }

        public async Task<ParkingZona?> GetByIdAsync(int id)
        {
            return await _context.ParkingZone.FindAsync(id);
        }

        public async Task AddAsync(ParkingZona parkingZona)
        {
            await _context.ParkingZone.AddAsync(parkingZona);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParkingZona parkingZona)
        {
            _context.ParkingZone.Update(parkingZona);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var parkingZona = await _context.ParkingZone.FindAsync(id);

            if (parkingZona != null)
            {
                _context.ParkingZone.Remove(parkingZona);
                await _context.SaveChangesAsync();
            }
        }
    }
}