using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class EmailObavijestRepository : IEmailObavijestRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailObavijestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmailObavijest>> GetAllAsync()
        {
            return await _context.EmailObavijesti.ToListAsync();
        }

        public async Task<EmailObavijest?> GetByIdAsync(int id)
        {
            return await _context.EmailObavijesti.FindAsync(id);
        }

        public async Task AddAsync(EmailObavijest emailObavijest)
        {
            await _context.EmailObavijesti.AddAsync(emailObavijest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailObavijest emailObavijest)
        {
            _context.EmailObavijesti.Update(emailObavijest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emailObavijest = await _context.EmailObavijesti.FindAsync(id);

            if (emailObavijest != null)
            {
                _context.EmailObavijesti.Remove(emailObavijest);
                await _context.SaveChangesAsync();
            }
        }
    }
}