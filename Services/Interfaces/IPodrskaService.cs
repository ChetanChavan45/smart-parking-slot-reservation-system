using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IPodrskaService
    {
        Task<List<PodrskaZahtjev>> GetZahtjeveAsync(
            string? korisnikId,
            bool mozePregledatiSve,
            string? status);

        Task<PodrskaZahtjev?> GetByIdAsync(int id);
        Task<PodrskaZahtjev> KreirajAsync(PodrskaZahtjev zahtjev, string korisnikId);
        Task<bool> OdgovoriAsync(int id, string odgovor, StatusPodrske status);
        Task DeleteAsync(int id);
        string FormatirajStatus(StatusPodrske status);
    }
}
