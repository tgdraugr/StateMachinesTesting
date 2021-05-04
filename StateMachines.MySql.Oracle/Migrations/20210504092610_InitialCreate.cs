using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StateMachines.MySql.Oracle.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Greetings",
                columns: table => new
                {
                    CorrelationId = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    CurrentState = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Sender = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    Occurrences = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Greetings", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Greetings");
        }
    }
}
