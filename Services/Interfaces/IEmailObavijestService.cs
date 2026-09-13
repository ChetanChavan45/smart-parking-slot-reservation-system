using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IEmailObavijestService
    {
        Task<List<EmailObavijest>> GetAllAsync();
        Task<EmailObavijest?> GetByIdAsync(int id);
        Task AddAsync(EmailObavijest emailObavijest);
        Task UpdateAsync(EmailObavijest emailObavijest);
        Task DeleteAsync(int id);
    }
}