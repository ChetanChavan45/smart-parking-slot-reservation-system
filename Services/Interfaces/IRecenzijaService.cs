using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IRecenzijaService
    {
        Task<List<Recenzija>> GetAllAsync();
        Task<Recenzija?> GetByIdAsync(int id);
        Task<Recenzija?> GetByRezervacijaIdAsync(int rezervacijaId);
        Task<bool> ExistsForRezervacijaAsync(int rezervacijaId);
        Task<RecenzijaEligibilityResult> CheckEligibilityAsync(int rezervacijaId, string korisnikId);
        Task<RecenzijaCreateResult> CreateForReservationAsync(
            int rezervacijaId,
            string korisnikId,
            Recenzija recenzija);
        Task AddAsync(Recenzija recenzija);
        Task UpdateAsync(Recenzija recenzija);
        Task DeleteAsync(int id);
    }

    public enum RecenzijaResultStatus
    {
        Success,
        NotFound,
        Forbidden,
        Invalid
    }

    public sealed class RecenzijaEligibilityResult
    {
        public RecenzijaResultStatus Status { get; init; }
        public string? ErrorMessage { get; init; }

        public bool Succeeded => Status == RecenzijaResultStatus.Success;

        public static RecenzijaEligibilityResult Success() => new()
        {
            Status = RecenzijaResultStatus.Success
        };

        public static RecenzijaEligibilityResult Failure(
            RecenzijaResultStatus status,
            string errorMessage) => new()
            {
                Status = status,
                ErrorMessage = errorMessage
            };
    }

    public sealed class RecenzijaCreateResult
    {
        public RecenzijaResultStatus Status { get; init; }
        public string? ErrorMessage { get; init; }

        public bool Succeeded => Status == RecenzijaResultStatus.Success;

        public static RecenzijaCreateResult Success() => new()
        {
            Status = RecenzijaResultStatus.Success
        };

        public static RecenzijaCreateResult Failure(
            RecenzijaResultStatus status,
            string errorMessage) => new()
            {
                Status = status,
                ErrorMessage = errorMessage
            };
    }
}
