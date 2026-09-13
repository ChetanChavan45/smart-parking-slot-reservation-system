using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class RecenzijaService : IRecenzijaService
    {
        private readonly IRecenzijaRepository _repository;
        private readonly IRezervacijaRepository _rezervacijaRepository;

        public RecenzijaService(
            IRecenzijaRepository repository,
            IRezervacijaRepository rezervacijaRepository)
        {
            _repository = repository;
            _rezervacijaRepository = rezervacijaRepository;
        }

        public Task<List<Recenzija>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Recenzija?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<Recenzija?> GetByRezervacijaIdAsync(int rezervacijaId)
        {
            return _repository.GetByRezervacijaIdAsync(rezervacijaId);
        }

        public Task<bool> ExistsForRezervacijaAsync(int rezervacijaId)
        {
            return _repository.ExistsForRezervacijaAsync(rezervacijaId);
        }

        public async Task<RecenzijaEligibilityResult> CheckEligibilityAsync(
            int rezervacijaId,
            string korisnikId)
        {
            var rezervacija = await _rezervacijaRepository.GetByIdAsync(rezervacijaId);

            if (rezervacija == null)
            {
                return RecenzijaEligibilityResult.Failure(
                    RecenzijaResultStatus.NotFound,
                    "Rezervacija nije pronađena.");
            }

            if (rezervacija.KorisnikId != korisnikId)
            {
                return RecenzijaEligibilityResult.Failure(
                    RecenzijaResultStatus.Forbidden,
                    "Nemate pravo ostaviti recenziju za ovu rezervaciju.");
            }

            if (rezervacija.StatusRezervacije != StatusRezervacije.Zavrsena)
            {
                return RecenzijaEligibilityResult.Failure(
                    RecenzijaResultStatus.Invalid,
                    "Recenziju možete ostaviti samo za završenu rezervaciju.");
            }

            if (await _repository.ExistsForRezervacijaAsync(rezervacijaId))
            {
                return RecenzijaEligibilityResult.Failure(
                    RecenzijaResultStatus.Invalid,
                    "Za ovu rezervaciju već postoji recenzija.");
            }

            return RecenzijaEligibilityResult.Success();
        }

        public async Task<RecenzijaCreateResult> CreateForReservationAsync(
            int rezervacijaId,
            string korisnikId,
            Recenzija recenzija)
        {
            var eligibility = await CheckEligibilityAsync(rezervacijaId, korisnikId);

            if (!eligibility.Succeeded)
            {
                return RecenzijaCreateResult.Failure(
                    eligibility.Status,
                    eligibility.ErrorMessage ?? "Recenziju nije moguće dodati.");
            }

            recenzija.Id = 0;
            recenzija.KorisnikId = korisnikId;
            recenzija.RezervacijaId = rezervacijaId;
            recenzija.Datum = DateTime.Now;
            recenzija.Obrisan = false;
            recenzija.Korisnik = null;
            recenzija.Rezervacija = null;

            await _repository.AddAsync(recenzija);

            return RecenzijaCreateResult.Success();
        }

        public Task AddAsync(Recenzija recenzija)
        {
            return _repository.AddAsync(recenzija);
        }

        public Task UpdateAsync(Recenzija recenzija)
        {
            return _repository.UpdateAsync(recenzija);
        }

        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
