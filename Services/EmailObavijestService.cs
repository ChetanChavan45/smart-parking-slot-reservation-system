using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class EmailObavijestService : IEmailObavijestService
    {
        private readonly IEmailObavijestRepository _repository;

        public EmailObavijestService(IEmailObavijestRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmailObavijest>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<EmailObavijest?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(EmailObavijest emailObavijest)
        {
            await _repository.AddAsync(emailObavijest);
        }

        public async Task UpdateAsync(EmailObavijest emailObavijest)
        {
            await _repository.UpdateAsync(emailObavijest);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}