using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class EmailObavijest
    {
        public int Id { get; set; }

        public string Primalac { get; set; } = string.Empty;
        public string Naslov { get; set; } = string.Empty;
        public string Sadrzaj { get; set; } = string.Empty;

        public DateTime DatumSlanja { get; set; }

        public TipObavijesti TipObavijesti { get; set; }
        public StatusEmaila StatusEmaila { get; set; }
    }
}