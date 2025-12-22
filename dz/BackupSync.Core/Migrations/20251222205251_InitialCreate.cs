using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupSync.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserGivenName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VolumeSerialNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastKnownLabel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastKnownLetter = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                });

            migrationBuilder.CreateTable(
                name: "SyncJobs",
                columns: table => new
                {
                    SyncJobId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SourcePath = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    DestPath = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    JobType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrphanPolicy = table.Column<int>(type: "INTEGER", nullable: false),
                    ConflictPolicy = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncJobs", x => x.SyncJobId);
                    table.ForeignKey(
                        name: "FK_SyncJobs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobExclusions",
                columns: table => new
                {
                    JobExclusionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pattern = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SyncJobId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExclusions", x => x.JobExclusionId);
                    table.ForeignKey(
                        name: "FK_JobExclusions_SyncJobs_SyncJobId",
                        column: x => x.SyncJobId,
                        principalTable: "SyncJobs",
                        principalColumn: "SyncJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobExclusions_SyncJobId",
                table: "JobExclusions",
                column: "SyncJobId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncJobs_DeviceId",
                table: "SyncJobs",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobExclusions");

            migrationBuilder.DropTable(
                name: "SyncJobs");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
