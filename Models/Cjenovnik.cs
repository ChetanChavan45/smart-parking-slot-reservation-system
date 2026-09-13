namespace PametniParkingSistem.Models
{
    public class Cjenovnik
    {
        public int Id { get; set; }

        public double CijenaPoSatu { get; set; }

        public DateTime VaziOd { get; set; }
        public DateTime VaziDo { get; set; }
    }
}