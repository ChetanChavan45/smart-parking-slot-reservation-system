using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class KriterijPretrage
    {
        public int Id { get; set; }

        public string Zona { get; set; } = string.Empty;

        public StatusMjesta Status { get; set; }
        public TipMjesta TipMjesta { get; set; }

        public bool Natkriveno { get; set; }

        public double MinCijena { get; set; }
        public double MaxCijena { get; set; }
    }
}