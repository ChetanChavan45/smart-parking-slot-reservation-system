using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IRezervacijaService
    {
        Task<List<Rezervacija>> GetAllAsync();
        Task<Rezervacija?> GetByIdAsync(int id);
        Task AddAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
        Task DeleteAsync(int id);
        Task<bool> ProvjeriDostupnostAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj);
        double IzracunajCijenu(DateTime pocetak, DateTime kraj, double cijenaPoSatu);
        Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId);
        Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync();

        Task<List<Rezervacija>> GetFiltriraneAsync(string? status);
        Task<List<Rezervacija>> GetFiltriraneZaKorisnikaAsync(string korisnikId, string? status);
        Task<List<Rezervacija>> GetHistorijuZaKorisnikaAsync(string korisnikId, string? status);
        Task AzurirajIstekleRezervacijeAsync();
        Task<RezervacijaCreateResult> PripremiKreiranjeAsync(int parkingMjestoId, string email);
        Task<RezervacijaCreateResult> ObradiKreiranjeAsync(Rezervacija rezervacija, string korisnikId);
        Task<RezervacijaEditResult> ObradiIzmjenuAsync(int id, Rezervacija izmjena);
        Task<RezervacijaCancelResult> OtkaziAsync(int id);
    }

    public class RezervacijaCreateResult
    {
        public bool Succeeded { get; init; }
        public Rezervacija? Rezervacija { get; init; }
        public ParkingMjesto? ParkingMjesto { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();

        public static RezervacijaCreateResult Success(
            Rezervacija rezervacija,
            ParkingMjesto parkingMjesto)
        {
            return new RezervacijaCreateResult
            {
                Succeeded = true,
                Rezervacija = rezervacija,
                ParkingMjesto = parkingMjesto
            };
        }

        public static RezervacijaCreateResult Failure(
            ParkingMjesto? parkingMjesto,
            params (string Key, string Message)[] errors)
        {
            return new RezervacijaCreateResult
            {
                Succeeded = false,
                ParkingMjesto = parkingMjesto,
                Errors = errors.ToDictionary(e => e.Key, e => e.Message)
            };
        }
    }

    public class RezervacijaEditResult
    {
        public bool Succeeded { get; init; }
        public bool RequiresPayment { get; init; }
        public Rezervacija? Rezervacija { get; init; }
        public double IznosDoplate { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();

        public static RezervacijaEditResult Updated(Rezervacija rezervacija)
        {
            return new RezervacijaEditResult
            {
                Succeeded = true,
                Rezervacija = rezervacija
            };
        }

        public static RezervacijaEditResult PaymentRequired(
            Rezervacija rezervacija,
            double iznosDoplate)
        {
            return new RezervacijaEditResult
            {
                Succeeded = true,
                RequiresPayment = true,
                Rezervacija = rezervacija,
                IznosDoplate = iznosDoplate
            };
        }

        public static RezervacijaEditResult Failure(
            Rezervacija? rezervacija,
            params (string Key, string Message)[] errors)
        {
            return new RezervacijaEditResult
            {
                Succeeded = false,
                Rezervacija = rezervacija,
                Errors = errors.ToDictionary(e => e.Key, e => e.Message)
            };
        }
    }

    public class RezervacijaCancelResult
    {
        public bool Succeeded { get; init; }
        public bool Refunded { get; init; }
        public Rezervacija? Rezervacija { get; init; }
        public string? Error { get; init; }

        public static RezervacijaCancelResult Success(
            Rezervacija rezervacija,
            bool refunded)
        {
            return new RezervacijaCancelResult
            {
                Succeeded = true,
                Rezervacija = rezervacija,
                Refunded = refunded
            };
        }

        public static RezervacijaCancelResult Failure(string error)
        {
            return new RezervacijaCancelResult
            {
                Succeeded = false,
                Error = error
            };
        }
    }
}
