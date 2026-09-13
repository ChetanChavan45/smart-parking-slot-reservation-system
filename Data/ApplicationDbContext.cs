using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rezervacija> Rezervacije { get; set; }
        public DbSet<ParkingMjesto> ParkingMjesta { get; set; }
        public DbSet<ParkingZona> ParkingZone { get; set; }
        public DbSet<Placanje> Placanja { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<EmailObavijest> EmailObavijesti { get; set; }
        public DbSet<Cjenovnik> Cjenovnici { get; set; }
        public DbSet<KriterijPretrage> KriterijiPretrage { get; set; }
        public DbSet<PodrskaZahtjev> PodrskaZahtjevi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
            modelBuilder.Entity<ParkingMjesto>().ToTable("ParkingMjesto");
            modelBuilder.Entity<ParkingZona>().ToTable("ParkingZona");
            modelBuilder.Entity<Placanje>().ToTable("Placanje");
            modelBuilder.Entity<Recenzija>().ToTable("Recenzija");
            modelBuilder.Entity<EmailObavijest>().ToTable("EmailObavijest");
            modelBuilder.Entity<Cjenovnik>().ToTable("Cjenovnik");
            modelBuilder.Entity<KriterijPretrage>().ToTable("KriterijPretrage");
            modelBuilder.Entity<PodrskaZahtjev>().ToTable("PodrskaZahtjev");

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Rezervacija)
                .WithOne(rz => rz.Recenzija)
                .HasForeignKey<Recenzija>(r => r.RezervacijaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Korisnik)
                .WithMany()
                .HasForeignKey(r => r.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rezervacija>()
                .HasOne(r => r.Korisnik)
                .WithMany(k => k.Rezervacije)
                .HasForeignKey(r => r.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PodrskaZahtjev>()
                .HasOne(p => p.Korisnik)
                .WithMany()
                .HasForeignKey(p => p.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}