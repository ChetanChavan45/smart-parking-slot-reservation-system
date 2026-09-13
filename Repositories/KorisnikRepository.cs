using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly ApplicationDbContext _context;

        public KorisnikRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Korisnik>> GetAllAsync()
        {
            return await _context.Users.ToListAsync(); 
        }

        public async Task<Korisnik?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _context.Users.AddAsync(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            _context.Users.Update(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var korisnik = await _context.Users.FindAsync(id);

            if (korisnik != null)
            {
                _context.Users.Remove(korisnik);
                await _context.SaveChangesAsync();
            }
        }
    }
}