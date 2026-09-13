using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.ViewModels.Placanje;

namespace PametniParkingSistem.Services
{
    public class PlacanjeService : IPlacanjeService
    {
        private readonly IPlacanjeRepository _placanjeRepository;
        private readonly IRezervacijaRepository _rezervacijaRepository;
        private readonly IParkingMjestoRepository _parkingMjestoRepository;
        private readonly IEmailSenderService _emailSender;

        public PlacanjeService(
            IPlacanjeRepository placanjeRepository,
            IRezervacijaRepository rezervacijaRepository,
            IParkingMjestoRepository parkingMjestoRepository,
            IEmailSenderService emailSender)
        {
            _placanjeRepository = placanjeRepository;
            _rezervacijaRepository = rezervacijaRepository;
            _parkingMjestoRepository = parkingMjestoRepository;
            _emailSender = emailSender;
        }

        public Task<List<Placanje>> GetAllAsync()
        {
            return _placanjeRepository.GetAllAsync();
        }

        public Task<Placanje?> GetByIdAsync(int id)
        {
            return _placanjeRepository.GetByIdAsync(id);
        }

        public Task AddAsync(Placanje placanje)
        {
            return _placanjeRepository.AddAsync(placanje);
        }

        public Task UpdateAsync(Placanje placanje)
        {
            return _placanjeRepository.UpdateAsync(placanje);
        }

        public Task DeleteAsync(int id)
        {
            return _placanjeRepository.DeleteAsync(id);
        }

        public Task<bool> PostojiPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return _placanjeRepository.PostojiPlacanjeZaRezervacijuAsync(rezervacijaId);
        }

        public Task<Placanje?> GetUspjesnoPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return _placanjeRepository.GetUspjesnoPlacanjeZaRezervacijuAsync(rezervacijaId);
        }

        public async Task<string> GetParkingMjestoNazivAsync(int parkingMjestoId)
        {
            var parkingMjesto = await _parkingMjestoRepository.GetByIdAsync(parkingMjestoId);

            return parkingMjesto == null
                ? $"Mjesto #{parkingMjestoId}"
                : $"{VratiNazivZone(parkingMjesto.ParkingZonaId)} - {parkingMjesto.Oznaka}";
        }

        public async Task<PlacanjeProcessResult> ObradiNovuRezervacijuAsync(
            Rezervacija rezervacija,
            PlacanjeRezervacijeViewModel model)
        {
            var validationError = ValidirajKarticnePodatke(model);
            if (validationError != null)
                return PlacanjeProcessResult.Failure(validationError);

            rezervacija.StatusRezervacije = StatusRezervacije.Aktivna;
            await _rezervacijaRepository.AddAsync(rezervacija);

            var placanje = KreirajPlacanje(
                model,
                rezervacija.UkupnaCijena,
                rezervacija.Id);

            await _placanjeRepository.AddAsync(placanje);

            var parkingMjesto = await _parkingMjestoRepository
                .GetByIdAsync(rezervacija.ParkingMjestoId);

            if (parkingMjesto != null)
            {
                parkingMjesto.Status = StatusMjesta.Rezervisano;
                await _parkingMjestoRepository.UpdateAsync(parkingMjesto);
            }

            var qrText =
                $"REZERVACIJA-{rezervacija.Id}-{parkingMjesto?.Oznaka}-{rezervacija.RegistracijskeTablice}";

            var qrUrl =
                "https://api.qrserver.com/v1/create-qr-code/?size=220x220&data=" +
                Uri.EscapeDataString(qrText);

            await _emailSender.SendEmailAsync(
                rezervacija.EmailZaObavijest,
                "Potvrda rezervacije - Pametni Parking Sistem",
                KreirajEmailTemplate(
                    "Potvrda rezervacije",
                    "Vaša rezervacija je uspješno potvrđena",
                    $@"
<p><b>Parking mjesto:</b> {parkingMjesto?.Oznaka}</p>
<p><b>Početak:</b> {rezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
<p><b>Kraj:</b> {rezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
<p><b>Registracijske tablice:</b> {rezervacija.RegistracijskeTablice}</p>
<p><b>Iznos plaćanja:</b> {rezervacija.UkupnaCijena:0.00} KM</p>
<p><b>Transakcijski broj:</b> {placanje.TransakcijskiBroj}</p>
<hr />
<h3 style='color:#166534;'>QR kod za ulazak</h3>
<p>Skenirajte ovaj QR kod prilikom ulaska na parking.</p>
<div style='text-align:center; margin-top:15px;'>
    <img src='{qrUrl}' alt='QR kod rezervacije' width='220' height='220' />
</div>
<p style='font-size:13px; color:#64748b; text-align:center;'>
    Kod rezervacije: {qrText}
</p>"));

            return PlacanjeProcessResult.Success();
        }

        public async Task<PlacanjeProcessResult> ObradiDoplatuAsync(
            Rezervacija izmjena,
            double iznosDoplate,
            PlacanjeRezervacijeViewModel model)
        {
            var validationError = ValidirajKarticnePodatke(model);
            if (validationError != null)
                return PlacanjeProcessResult.Failure(validationError);

            var postojecaRezervacija = await _rezervacijaRepository
                .GetByIdAsync(izmjena.Id);

            if (postojecaRezervacija == null)
                return PlacanjeProcessResult.Failure("Rezervacija nije pronađena.");

            postojecaRezervacija.VrijemePocetka = izmjena.VrijemePocetka;
            postojecaRezervacija.VrijemeKraja = izmjena.VrijemeKraja;
            postojecaRezervacija.RegistracijskeTablice = izmjena.RegistracijskeTablice;
            postojecaRezervacija.KontaktTelefon = izmjena.KontaktTelefon;
            postojecaRezervacija.EmailZaObavijest = izmjena.EmailZaObavijest;
            postojecaRezervacija.UkupnaCijena = izmjena.UkupnaCijena;
            postojecaRezervacija.StatusRezervacije = StatusRezervacije.Aktivna;

            await _rezervacijaRepository.UpdateAsync(postojecaRezervacija);

            var placanje = KreirajPlacanje(
                model,
                iznosDoplate,
                postojecaRezervacija.Id);

            await _placanjeRepository.AddAsync(placanje);

            await _emailSender.SendEmailAsync(
                postojecaRezervacija.EmailZaObavijest,
                "Potvrda doplate - Pametni Parking Sistem",
                KreirajEmailTemplate(
                    "Potvrda doplate",
                    "Doplata je uspješno izvršena",
                    $@"
<p><b>Rezervacija je ažurirana.</b></p>
<p><b>Novi početak:</b> {postojecaRezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
<p><b>Novi kraj:</b> {postojecaRezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
<p><b>Doplaćeni iznos:</b> {iznosDoplate:0.00} KM</p>
<p><b>Ukupna cijena rezervacije:</b> {postojecaRezervacija.UkupnaCijena:0.00} KM</p>
<p><b>Transakcijski broj:</b> {placanje.TransakcijskiBroj}</p>"));

            return PlacanjeProcessResult.Success();
        }

        private static Placanje KreirajPlacanje(
            PlacanjeRezervacijeViewModel model,
            double iznos,
            int rezervacijaId)
        {
            var brojKartice = model.BrojKartice.Replace(" ", string.Empty);
            var zadnje4 = brojKartice[^4..];

            return new Placanje
            {
                ImeVlasnikaKartice = model.ImeVlasnikaKartice,
                BrojKarticeMaskiran = $"**** **** **** {zadnje4}",
                DatumPlacanja = DateTime.Now,
                Iznos = iznos,
                StatusPlacanja = StatusPlacanja.Uspjesno,
                TransakcijskiBroj = Guid.NewGuid()
                    .ToString()[..8]
                    .ToUpperInvariant(),
                RezervacijaId = rezervacijaId
            };
        }

        private static string? ValidirajKarticnePodatke(
            PlacanjeRezervacijeViewModel model)
        {
            var brojKartice = model.BrojKartice?.Replace(" ", string.Empty)
                ?? string.Empty;

            if (brojKartice.Length < 12)
                return "Broj kartice nije ispravan.";

            if (string.IsNullOrWhiteSpace(model.CVV) || model.CVV.Length < 3)
                return "CVV nije ispravan.";

            return null;
        }

        private static string VratiNazivZone(int parkingZonaId)
        {
            return parkingZonaId switch
            {
                1 => "Zona A",
                2 => "Zona B",
                3 => "Zona C",
                4 => "VIP zona",
                _ => $"Zona #{parkingZonaId}"
            };
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
