using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PametniParkingSistem.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanaRecenzijaZaRezervaciju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Recenzija",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RezervacijaId",
                table: "Recenzija",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_ParkingMjestoId",
                table: "Rezervacija",
                column: "ParkingMjestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzija_KorisnikId",
                table: "Recenzija",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzija_RezervacijaId",
                table: "Recenzija",
                column: "RezervacijaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Placanje_RezervacijaId",
                table: "Placanje",
                column: "RezervacijaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Placanje_Rezervacija_RezervacijaId",
                table: "Placanje",
                column: "RezervacijaId",
                principalTable: "Rezervacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzija_AspNetUsers_KorisnikId",
                table: "Recenzija",
                column: "KorisnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija",
                column: "RezervacijaId",
                principalTable: "Rezervacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_AspNetUsers_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_ParkingMjesto_ParkingMjestoId",
                table: "Rezervacija",
                column: "ParkingMjestoId",
                principalTable: "ParkingMjesto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Placanje_Rezervacija_RezervacijaId",
                table: "Placanje");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzija_AspNetUsers_KorisnikId",
                table: "Recenzija");

            migrationBuilder.DropForeignKey(
                name: "FK_Recenzija_Rezervacija_RezervacijaId",
                table: "Recenzija");

            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_AspNetUsers_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_ParkingMjesto_ParkingMjestoId",
                table: "Rezervacija");

            migrationBuilder.DropIndex(
                name: "IX_Rezervacija_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.DropIndex(
                name: "IX_Rezervacija_ParkingMjestoId",
                table: "Rezervacija");

            migrationBuilder.DropIndex(
                name: "IX_Recenzija_KorisnikId",
                table: "Recenzija");

            migrationBuilder.DropIndex(
                name: "IX_Recenzija_RezervacijaId",
                table: "Recenzija");

            migrationBuilder.DropIndex(
                name: "IX_Placanje_RezervacijaId",
                table: "Placanje");

            migrationBuilder.DropColumn(
                name: "RezervacijaId",
                table: "Recenzija");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikId",
                table: "Recenzija",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
