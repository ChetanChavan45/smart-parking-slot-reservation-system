using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class RezervacijaService : IRezervacijaService
    {
        private const double PdvStopa = 0.17;

        private readonly IRezervacijaRepository _repository;
        private readonly IParkingMjestoService _parkingMjestoService;
        private readonly IPlacanjeService _placanjeService;
        private readonly IEmailSenderService _emailSender;

        public RezervacijaService(
            IRezervacijaRepository repository,
            IParkingMjestoService parkingMjestoService,
            IPlacanjeService placanjeService,
            IEmailSenderService emailSender)
        {
            _repository = repository;
            _parkingMjestoService = parkingMjestoService;
            _placanjeService = placanjeService;
            _emailSender = emailSender;
        }

        public Task<List<Rezervacija>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Rezervacija?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task AddAsync(Rezervacija rezervacija)
        {
            return _repository.AddAsync(rezervacija);
        }

        public Task UpdateAsync(Rezervacija rezervacija)
        {
            return _repository.UpdateAsync(rezervacija);
        }

        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        public async Task<bool> ProvjeriDostupnostAsync(
            int parkingMjestoId,
            DateTime pocetak,
            DateTime kraj)
        {
            var postojiPreklapanje = await _repository
                .PostojiPreklapanjeTerminaAsync(parkingMjestoId, pocetak, kraj);

            return !postojiPreklapanje;
        }

        public double IzracunajCijenu(
            DateTime pocetak,
            DateTime kraj,
            double cijenaPoSatu)
        {
            var trajanjeUSatima = (kraj - pocetak).TotalHours;

            if (trajanjeUSatima <= 0)
                return 0;

            return Math.Ceiling(trajanjeUSatima) * cijenaPoSatu;
        }

        public Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId)
        {
            return _repository.GetByKorisnikIdAsync(korisnikId);
        }

        public Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync()
        {
            return _repository.GetIstekleAktivneRezervacijeAsync();
        }

        public async Task<List<Rezervacija>> GetFiltriraneAsync(string? status)
        {
            await AzurirajIstekleRezervacijeAsync();
            var rezervacije = await _repository.GetAllAsync();

            return FiltrirajPoStatusu(rezervacije, status)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToList();
        }

        public async Task<List<Rezervacija>> GetFiltriraneZaKorisnikaAsync(
            string korisnikId,
            string? status)
        {
            await AzurirajIstekleRezervacijeAsync();
            var rezervacije = await _repository.GetByKorisnikIdAsync(korisnikId);

            return FiltrirajPoStatusu(rezervacije, status)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToList();
        }

        public async Task<List<Rezervacija>> GetHistorijuZaKorisnikaAsync(
            string korisnikId,
            string? status)
        {
            await AzurirajIstekleRezervacijeAsync();
            var rezervacije = await _repository.GetByKorisnikIdAsync(korisnikId);

            var historija = rezervacije.Where(r =>
                r.StatusRezervacije == StatusRezervacije.Zavrsena ||
                r.StatusRezervacije == StatusRezervacije.Otkazana);

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StatusRezervacije>(status, out var parsedStatus))
            {
                historija = historija.Where(r =>
                    r.StatusRezervacije == parsedStatus);
            }

            return historija
                .OrderByDescending(r => r.DatumKreiranja)
                .ToList();
        }

        public async Task AzurirajIstekleRezervacijeAsync()
        {
            var istekleRezervacije =
                await _repository.GetIstekleAktivneRezervacijeAsync();

            foreach (var rezervacija in istekleRezervacije)
            {
                rezervacija.StatusRezervacije = StatusRezervacije.Zavrsena;
                await _repository.UpdateAsync(rezervacija);
            }
        }

        public async Task<RezervacijaCreateResult> PripremiKreiranjeAsync(
            int parkingMjestoId,
            string email)
        {
            var parkingMjesto =
                await _parkingMjestoService.GetByIdAsync(parkingMjestoId);

            if (parkingMjesto == null)
            {
                return RezervacijaCreateResult.Failure(
                    null,
                    (string.Empty, "Parking mjesto nije pronađeno."));
            }

            var rezervacija = new Rezervacija
            {
                ParkingMjestoId = parkingMjestoId,
                DatumKreiranja = DateTime.Now,
                VrijemePocetka = DateTime.Now,
                VrijemeKraja = DateTime.Now.AddHours(1),
                EmailZaObavijest = email
            };

            return RezervacijaCreateResult.Success(rezervacija, parkingMjesto);
        }

        public async Task<RezervacijaCreateResult> ObradiKreiranjeAsync(
            Rezervacija rezervacija,
            string korisnikId)
        {
            var parkingMjesto = await _parkingMjestoService
                .GetByIdAsync(rezervacija.ParkingMjestoId);

            if (parkingMjesto == null)
            {
                return RezervacijaCreateResult.Failure(
                    null,
                    (string.Empty, "Parking mjesto nije pronađeno."));
            }

            var errors = ValidirajTermin(
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja,
                provjeriProslost: true);

            if (errors.Count > 0)
            {
                return new RezervacijaCreateResult
                {
                    Succeeded = false,
                    Rezervacija = rezervacija,
                    ParkingMjesto = parkingMjesto,
                    Errors = errors
                };
            }

            var dostupno = await ProvjeriDostupnostAsync(
                rezervacija.ParkingMjestoId,
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja);

            if (!dostupno)
            {
                return RezervacijaCreateResult.Failure(
                    parkingMjesto,
                    (string.Empty,
                        "Parking mjesto nije dostupno u odabranom terminu."));
            }

            rezervacija.KorisnikId = korisnikId;
            rezervacija.DatumKreiranja = DateTime.Now;
            rezervacija.StatusRezervacije = StatusRezervacije.Kreirana;
            rezervacija.UkupnaCijena = IzracunajCijenuSaPdvom(
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja,
                parkingMjesto.CijenaPoSatu);

            return RezervacijaCreateResult.Success(rezervacija, parkingMjesto);
        }

        public async Task<RezervacijaEditResult> ObradiIzmjenuAsync(
            int id,
            Rezervacija izmjena)
        {
            var postojeca = await _repository.GetByIdAsync(id);

            if (postojeca == null)
            {
                return RezervacijaEditResult.Failure(
                    null,
                    (string.Empty, "Rezervacija nije pronađena."));
            }

            var parkingMjesto = await _parkingMjestoService
                .GetByIdAsync(postojeca.ParkingMjestoId);

            if (parkingMjesto == null)
            {
                return RezervacijaEditResult.Failure(
                    postojeca,
                    (string.Empty, "Parking mjesto nije pronađeno."));
            }

            var errors = ValidirajTermin(
                izmjena.VrijemePocetka,
                izmjena.VrijemeKraja,
                provjeriProslost: false);

            if (errors.Count > 0)
            {
                return new RezervacijaEditResult
                {
                    Succeeded = false,
                    Rezervacija = izmjena,
                    Errors = errors
                };
            }

            var novaCijena = IzracunajCijenuSaPdvom(
                izmjena.VrijemePocetka,
                izmjena.VrijemeKraja,
                parkingMjesto.CijenaPoSatu);

            var razlikaZaDoplatu = novaCijena - postojeca.UkupnaCijena;

            PrimijeniIzmjenu(postojeca, izmjena, novaCijena);

            if (razlikaZaDoplatu > 0)
            {
                return RezervacijaEditResult.PaymentRequired(
                    postojeca,
                    razlikaZaDoplatu);
            }

            await _repository.UpdateAsync(postojeca);
            return RezervacijaEditResult.Updated(postojeca);
        }

        public async Task<RezervacijaCancelResult> OtkaziAsync(int id)
        {
            var rezervacija = await _repository.GetByIdAsync(id);

            if (rezervacija == null)
            {
                return RezervacijaCancelResult.Failure(
                    "Rezervacija nije pronađena.");
            }

            rezervacija.StatusRezervacije = StatusRezervacije.Otkazana;
            await _repository.UpdateAsync(rezervacija);

            var placanje = await _placanjeService
                .GetUspjesnoPlacanjeZaRezervacijuAsync(rezervacija.Id);

            var refunded = false;

            if (placanje != null)
            {
                placanje.StatusPlacanja = StatusPlacanja.Refundirano;
                await _placanjeService.UpdateAsync(placanje);
                refunded = true;
            }

            if (!string.IsNullOrWhiteSpace(rezervacija.EmailZaObavijest))
            {
                await _emailSender.SendEmailAsync(
                    rezervacija.EmailZaObavijest,
                    "Rezervacija otkazana - Pametni Parking Sistem",
                    KreirajEmailTemplate(
                        "Otkazivanje rezervacije",
                        "Vaša rezervacija je otkazana",
                        $@"
                        <p><b>Početak:</b> {rezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
                        <p><b>Kraj:</b> {rezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
                        <p><b>Registracijske tablice:</b> {rezervacija.RegistracijskeTablice}</p>
                        <p><b>Iznos:</b> {rezervacija.UkupnaCijena:0.00} KM</p>
                        <p><b>Status plaćanja:</b> {(refunded ? "Refundirano" : "Nema evidentiranog plaćanja")}</p>",
                        refunded
                            ? "Novac je evidentiran kao refundiran u sistemu."
                            : "Za rezervaciju nije pronađeno uspješno plaćanje."));
            }

            return RezervacijaCancelResult.Success(rezervacija, refunded);
        }

        private static IEnumerable<Rezervacija> FiltrirajPoStatusu(
            IEnumerable<Rezervacija> rezervacije,
            string? status)
        {
            return status switch
            {
                "Aktivne" => rezervacije.Where(r =>
                    r.StatusRezervacije == StatusRezervacije.Aktivna ||
                    r.StatusRezervacije == StatusRezervacije.Produzena ||
                    r.StatusRezervacije == StatusRezervacije.Kreirana),

                "Zavrsene" => rezervacije.Where(r =>
                    r.StatusRezervacije == StatusRezervacije.Zavrsena),

                "Otkazane" => rezervacije.Where(r =>
                    r.StatusRezervacije == StatusRezervacije.Otkazana),

                _ => rezervacije
            };
        }

        private static Dictionary<string, string> ValidirajTermin(
            DateTime pocetak,
            DateTime kraj,
            bool provjeriProslost)
        {
            var errors = new Dictionary<string, string>();

            if (provjeriProslost && pocetak < DateTime.Now)
            {
                errors["VrijemePocetka"] =
                    "Datum i vrijeme početka rezervacije ne mogu biti u prošlosti.";
            }

            if (kraj <= pocetak)
            {
                errors[string.Empty] =
                    "Vrijeme kraja mora biti nakon vremena početka.";
            }

            return errors;
        }

        private double IzracunajCijenuSaPdvom(
            DateTime pocetak,
            DateTime kraj,
            double cijenaPoSatu)
        {
            var osnovica = IzracunajCijenu(pocetak, kraj, cijenaPoSatu);
            return osnovica + osnovica * PdvStopa;
        }

        private static void PrimijeniIzmjenu(
            Rezervacija postojeca,
            Rezervacija izmjena,
            double novaCijena)
        {
            postojeca.VrijemePocetka = izmjena.VrijemePocetka;
            postojeca.VrijemeKraja = izmjena.VrijemeKraja;
            postojeca.RegistracijskeTablice = izmjena.RegistracijskeTablice;
            postojeca.KontaktTelefon = izmjena.KontaktTelefon;
            postojeca.EmailZaObavijest = izmjena.EmailZaObavijest;
            postojeca.UkupnaCijena = novaCijena;
            postojeca.StatusRezervacije = StatusRezervacije.Aktivna;
        }

        private static string KreirajEmailTemplate(
            string naslov,
            string poruka,
            string sadrzaj,
            string footer = "Hvala što koristite Pametni Parking Sistem.")
        {
            return $@"
            <div style='font-family:Segoe UI, Arial, sans-serif; background:#f4f8f5; padding:30px; color:#1f2937;'>
                <div style='max-width:650px; margin:auto; background:white; border-radius:18px; overflow:hidden; box-shadow:0 8px 24px rgba(0,0,0,0.08);'>
                    <div style='background:linear-gradient(90deg,#1f7a4d,#2ea86b); padding:26px; color:white;'>
                        <h1 style='margin:0; font-size:26px;'>Pametni Parking Sistem</h1>
                        <p style='margin:6px 0 0 0; opacity:0.9;'>{naslov}</p>
                    </div>
                    <div style='padding:30px;'>
                        <h2 style='color:#166534; margin-top:0;'>{poruka}</h2>
                        <div style='background:#f8faf9; border:1px solid #e5e7eb; border-radius:14px; padding:20px; margin:20px 0;'>
                            {sadrzaj}
                        </div>
                        <p style='font-size:14px; color:#64748b;'>{footer}</p>
                    </div>
                    <div style='background:#f1f5f3; padding:18px; text-align:center; font-size:13px; color:#64748b;'>
                        Ovo je automatska poruka. Molimo ne odgovarajte na ovaj email.
                    </div>
                </div>
            </div>";
        }
    }
}
