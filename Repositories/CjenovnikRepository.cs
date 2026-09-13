using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class CjenovnikRepository : ICjenovnikRepository
    {
        private readonly ApplicationDbContext _context;

        public CjenovnikRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cjenovnik>> GetAllAsync()
        {
            return await _context.Cjenovnici.ToListAsync();
        }

        public async Task<Cjenovnik?> GetByIdAsync(int id)
        {
            return await _context.Cjenovnici.FindAsync(id);
        }

        public async Task AddAsync(Cjenovnik cjenovnik)
        {
            await _context.Cjenovnici.AddAsync(cjenovnik);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cjenovnik cjenovnik)
        {
            _context.Cjenovnici.Update(cjenovnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cjenovnik = await _context.Cjenovnici.FindAsync(id);

            if (cjenovnik != null)
            {
                _context.Cjenovnici.Remove(cjenovnik);
                await _context.SaveChangesAsync();
            }
        }
    }
}