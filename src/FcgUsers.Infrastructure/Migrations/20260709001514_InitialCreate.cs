using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcgUsers.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Name = table.Column<string>(type: "character varying(150)", unicode: false, maxLength: 150, nullable: false),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Email = table.Column<string>(type: "character varying(254)", unicode: false, maxLength: 254, nullable: false),
                Password = table.Column<string>(type: "character varying(60)", unicode: false, maxLength: 60, nullable: false),
                Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                IsInactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");
    }
}
