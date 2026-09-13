using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDashboardData> GetDashboardDataAsync(Korisnik? prijavljeniKorisnik);
    }

    public class HomeDashboardData
    {
        public double ProsjecnaOcjena { get; init; }
        public int BrojRecenzija { get; init; }
        public List<Recenzija> ZadnjeRecenzije { get; init; } = new();

        public int BrojRezervacija { get; init; }
        public int AktivneRezervacije { get; init; }
        public int ZavrseneRezervacije { get; init; }
        public int OtkazaneRezervacije { get; init; }
        public List<Rezervacija> ZadnjeRezervacije { get; init; } = new();

        public int BrojParkingMjesta { get; init; }
        public List<ParkingMjesto> MjestaVanFunkcije { get; init; } = new();

        public int BrojKorisnika { get; init; }
        public List<Korisnik> NoviKorisnici { get; init; } = new();

        public decimal UkupnaZarada { get; init; }
        public List<Placanje> ZadnjaPlacanja { get; init; } = new();

        public int MojeRezervacijeCount { get; init; }
        public Rezervacija? MojaAktivnaRezervacija { get; init; }
    }
}
