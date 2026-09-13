using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IEmailObavijestRepository
    {
        Task<List<EmailObavijest>> GetAllAsync();
        Task<EmailObavijest?> GetByIdAsync(int id);
        Task AddAsync(EmailObavijest emailObavijest);
        Task UpdateAsync(EmailObavijest emailObavijest);
        Task DeleteAsync(int id);
    }
}