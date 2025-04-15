using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PCC.Migrations
{
    public partial class updatetransfermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_TransferModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Transfer_Number = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AnimalId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Telephone_Number = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Mobile_Number = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Email = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Transfer_File = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "date", nullable: true),
                    Date_Updated = table.Column<DateTime>(type: "date", nullable: true),
                    Delete_Flag = table.Column<bool>(type: "bit", nullable: false),
                    Created_By = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Updated_By = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Date_Deleted = table.Column<DateTime>(type: "date", nullable: true),
                    Deleted_By = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Date_Restored = table.Column<DateTime>(type: "date", nullable: true),
                    Restored_By = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_TransferModel_A_Buff_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "A_Buff_Animal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_TransferModel_tbl_FarmOwner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Tbl_Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_TransferModel_AnimalId",
                table: "tbl_TransferModel",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_TransferModel_OwnerId",
                table: "tbl_TransferModel",
                column: "OwnerId");

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
