using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PametniParkingSistem.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanaPodrskaZahtjevi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija");

            migrationBuilder.AlterColumn<string>(
                name: "RegistracijskeTablice",
                table: "Rezervacija",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "PodrskaZahtjev",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naslov = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Kategorija = table.Column<int>(type: "int", nullable: false),
                    Prioritet = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumOdgovora = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Odgovor = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PodrskaZahtjev", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PodrskaZahtjev_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PodrskaZahtjev_KorisnikId",
                table: "PodrskaZahtjev",
                column: "KorisnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija",
                column: "RezervacijaId",
                principalTable: "Rezervacija",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija");

            migrationBuilder.DropTable(
                name: "PodrskaZahtjev");

            migrationBuilder.AlterColumn<string>(
                name: "RegistracijskeTablice",
                table: "Rezervacija",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija",
                column: "RezervacijaId",
                principalTable: "Rezervacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
