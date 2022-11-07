using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
	/// <inheritdoc />
	public partial class AddedPhoto : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Photos",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint", nullable: false),
					PostId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Photos", x => x.Id);
					table.ForeignKey(
						name: "FK_Photos_Attaches_Id",
						column: x => x.Id,
						principalTable: "Attaches",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Photos_Posts_PostId",
						column: x => x.PostId,
						principalTable: "Posts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Photos_PostId",
				table: "Photos",
				column: "PostId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Photos");
		}
	}
}