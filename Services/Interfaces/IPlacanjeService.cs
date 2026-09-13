using PametniParkingSistem.Models;
using PametniParkingSistem.ViewModels.Placanje;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IPlacanjeService
    {
        Task<List<Placanje>> GetAllAsync();
        Task<Placanje?> GetByIdAsync(int id);
        Task AddAsync(Placanje placanje);
        Task UpdateAsync(Placanje placanje);
        Task DeleteAsync(int id);
        Task<bool> PostojiPlacanjeZaRezervacijuAsync(int rezervacijaId);
        Task<Placanje?> GetUspjesnoPlacanjeZaRezervacijuAsync(int rezervacijaId);

        Task<string> GetParkingMjestoNazivAsync(int parkingMjestoId);
        Task<PlacanjeProcessResult> ObradiNovuRezervacijuAsync(
            Rezervacija rezervacija,
            PlacanjeRezervacijeViewModel model);
        Task<PlacanjeProcessResult> ObradiDoplatuAsync(
            Rezervacija izmjena,
            double iznosDoplate,
            PlacanjeRezervacijeViewModel model);
    }

    public class PlacanjeProcessResult
    {
        public bool Succeeded { get; init; }
        public string? Error { get; init; }

        public static PlacanjeProcessResult Success()
        {
            return new PlacanjeProcessResult { Succeeded = true };
        }

        public static PlacanjeProcessResult Failure(string error)
        {
            return new PlacanjeProcessResult
            {
                Succeeded = false,
                Error = error
            };
        }
    }
}
