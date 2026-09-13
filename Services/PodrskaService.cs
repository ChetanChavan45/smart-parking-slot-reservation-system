using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class PodrskaService : IPodrskaService
    {
        private readonly IPodrskaRepository _repository;

        public PodrskaService(IPodrskaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PodrskaZahtjev>> GetZahtjeveAsync(
            string? korisnikId,
            bool mozePregledatiSve,
            string? status)
        {
            StatusPodrske? parsedStatus = null;

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StatusPodrske>(status, out var value))
            {
                parsedStatus = value;
            }

            if (mozePregledatiSve)
                return await _repository.GetAllAsync(parsedStatus);

            if (string.IsNullOrWhiteSpace(korisnikId))
                return new List<PodrskaZahtjev>();

            return await _repository.GetByKorisnikIdAsync(korisnikId, parsedStatus);
        }

        public Task<PodrskaZahtjev?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<PodrskaZahtjev> KreirajAsync(
            PodrskaZahtjev zahtjev,
            string korisnikId)
        {
            zahtjev.KorisnikId = korisnikId;
            zahtjev.Status = StatusPodrske.Otvoren;
            zahtjev.DatumKreiranja = DateTime.Now;
            zahtjev.DatumOdgovora = null;
            zahtjev.Odgovor = null;

            await _repository.AddAsync(zahtjev);
            return zahtjev;
        }

        public async Task<bool> OdgovoriAsync(
            int id,
            string odgovor,
            StatusPodrske status)
        {
            if (string.IsNullOrWhiteSpace(odgovor))
                return false;

            var zahtjev = await _repository.GetByIdAsync(id);

            if (zahtjev == null)
                return false;

            zahtjev.Odgovor = odgovor.Trim();
            zahtjev.Status = status;
            zahtjev.DatumOdgovora = DateTime.Now;

            await _repository.UpdateAsync(zahtjev);
            return true;
        }

        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        public string FormatirajStatus(StatusPodrske status)
        {
            return status switch
            {
                StatusPodrske.Otvoren => "Otvoren",
                StatusPodrske.UObradi => "U obradi",
                StatusPodrske.Rijesen => "Riješen",
                StatusPodrske.Zatvoren => "Zatvoren",
                _ => status.ToString()
            };
        }
    }
}
