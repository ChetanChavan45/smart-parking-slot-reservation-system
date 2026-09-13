using PametniParkingSistem.Models;
using PametniParkingSistem.ViewModels.Account;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountServiceResult> RegisterAsync(RegisterViewModel model);

        Task<AccountServiceResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }

    public class AccountServiceResult
    {
        public bool Succeeded { get; init; }

        public Korisnik? Korisnik { get; init; }

        public IReadOnlyCollection<string> Errors { get; init; }
            = Array.Empty<string>();

        public static AccountServiceResult Success(Korisnik korisnik)
        {
            return new AccountServiceResult
            {
                Succeeded = true,
                Korisnik = korisnik
            };
        }

        public static AccountServiceResult Failure(params string[] errors)
        {
            return new AccountServiceResult
            {
                Succeeded = false,
                Errors = errors
            };
        }

        public static AccountServiceResult Failure(IEnumerable<string> errors)
        {
            return new AccountServiceResult
            {
                Succeeded = false,
                Errors = errors.ToArray()
            };
        }
    }
}