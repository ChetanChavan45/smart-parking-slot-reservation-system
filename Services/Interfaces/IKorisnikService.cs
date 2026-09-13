using Microsoft.AspNetCore.Http;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IKorisnikService
    {
        Task<List<Korisnik>> GetAllAsync();
        Task<List<Korisnik>> GetVisibleUsersAsync(bool isAdministrator);
        Task<Korisnik?> GetAccessibleByIdAsync(string id, bool isAdministrator);
        Task<Korisnik?> GetByIdAsync(string id);
        Task<int> GetReservationCountAsync(string userId);
        Task<KorisnikServiceResult> CreateAsync(Korisnik korisnik);
        Task<KorisnikServiceResult> UpdateAccountAsync(string id, StatusNaloga statusNaloga, Uloga? uloga, bool isAdministrator);
        Task<KorisnikServiceResult> UpdateProfileAsync(string userId, Korisnik model, IFormFile? profilnaSlika);
        Task<KorisnikServiceResult> ChangePasswordAsync(string userId, string trenutnaLozinka, string novaLozinka);
        Task<KorisnikServiceResult> SetStatusAsync(string id, StatusNaloga status, bool isAdministrator);
        Task DeleteAsync(string id);
    }

    public class KorisnikServiceResult
    {
        public bool Succeeded { get; init; }
        public bool NotFound { get; init; }
        public bool Forbidden { get; init; }
        public Korisnik? Korisnik { get; init; }
        public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();

        public static KorisnikServiceResult Success(Korisnik? korisnik = null) =>
            new() { Succeeded = true, Korisnik = korisnik };

        public static KorisnikServiceResult Failure(params string[] errors) =>
            new() { Errors = errors };

        public static KorisnikServiceResult Missing() => new() { NotFound = true };
        public static KorisnikServiceResult Denied() => new() { Forbidden = true };
    }
}
