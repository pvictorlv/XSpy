using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XSpy.Database.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ranks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ranks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    name = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    username = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    password = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: true),
                    fullname = table.Column<string>(type: "varchar(140) CHARACTER SET utf8mb4", maxLength: 140, nullable: true),
                    rank_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    device_token = table.Column<Guid>(type: "char(36)", nullable: false),
                    email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_ranks_rank_id",
                        column: x => x.rank_id,
                        principalTable: "ranks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rank_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    rank_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    fake_role = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    role_name = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rank_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rank_roles_ranks_rank_id",
                        column: x => x.rank_id,
                        principalTable: "ranks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rank_roles_roles_role_name",
                        column: x => x.role_name,
                        principalTable: "roles",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    device_id = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: true),
                    model = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: true),
                    manufacturer = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: true),
                    sys_version = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    last_ip = table.Column<string>(type: "varchar(45) CHARACTER SET utf8mb4", maxLength: 45, nullable: true),
                    added_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.id);
                    table.ForeignKey(
                        name: "FK_devices_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_calls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    number = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    name = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", maxLength: 255, nullable: true),
                    duration = table.Column<string>(type: "varchar(10) CHARACTER SET utf8mb4", maxLength: 10, nullable: true),
                    device_date = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    call_type = table.Column<int>(type: "int", nullable: false),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_calls", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_calls_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_clipboard_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    content = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_clipboard_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_clipboard_logs_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    number = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    contact_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_contacts_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    original_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    file_path = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_files_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_installed_apps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    app_name = table.Column<string>(type: "varchar(120) CHARACTER SET utf8mb4", maxLength: 120, nullable: true),
                    package_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    version_name = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: true),
                    version_code = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_installed_apps", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_installed_apps_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    altitude = table.Column<double>(type: "double", nullable: false),
                    accuracy = table.Column<double>(type: "double", nullable: false),
                    speed = table.Column<double>(type: "double", nullable: false),
                    is_enabled = table.Column<double>(type: "double", nullable: false),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_locations_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    key = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    content = table.Column<string>(type: "varchar(500) CHARACTER SET utf8mb4", maxLength: 500, nullable: true),
                    device_date = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    call_type = table.Column<int>(type: "int", nullable: false),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_notifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    key = table.Column<string>(type: "varchar(120) CHARACTER SET utf8mb4", maxLength: 120, nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_permissions_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_sms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    address = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    body = table.Column<string>(type: "varchar(180) CHARACTER SET utf8mb4", maxLength: 180, nullable: true),
                    device_date = table.Column<string>(type: "varchar(30) CHARACTER SET utf8mb4", maxLength: 30, nullable: true),
                    call_type = table.Column<int>(type: "int", nullable: false),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_sms", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_sms_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_voice_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    original_name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    file_path = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_voice_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_voice_records_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_wifi",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    bssid = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ssid = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_wifi", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_wifi_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ranks",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("4288aa01-036a-47e4-9db8-61e425ac2d43"), "Usuário" },
                    { new Guid("62a840f9-c6ef-4d56-8652-4d9b46115b95"), "Admin" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "name", "title" },
                values: new object[,]
                {
                    { "IS_ADMIN", "É um administrador geral (privilégios globais)" },
                    { "ROLE_C_USER", "Pode criar usuários" },
                    { "ROLE_R_USER", "Pode ler usuários" },
                    { "ROLE_U_USER", "Pode editar usuários" },
                    { "ROLE_D_USER", "Pode deletar usuários" },
                    { "ROLE_C_ROLE", "Pode criar permissões" },
                    { "ROLE_R_ROLE", "Pode ler permissões" },
                    { "ROLE_U_ROLE", "Pode editar permissões" },
                    { "ROLE_D_ROLE", "Pode deletar permissões" },
                    { "IS_NORMAL_USER", "É um usuário comum" }
                });

            migrationBuilder.InsertData(
                table: "rank_roles",
                columns: new[] { "Id", "fake_role", "rank_id", "role_name" },
                values: new object[] { new Guid("817285cb-e22e-4bfe-b5e0-bc8d603ea57f"), false, new Guid("62a840f9-c6ef-4d56-8652-4d9b46115b95"), "IS_ADMIN" });

            migrationBuilder.InsertData(
                table: "rank_roles",
                columns: new[] { "Id", "fake_role", "rank_id", "role_name" },
                values: new object[] { new Guid("c18d9d6e-8be7-4dee-9d26-f442cb1dc0fc"), false, new Guid("4288aa01-036a-47e4-9db8-61e425ac2d43"), "IS_NORMAL_USER" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "device_token", "email", "is_active", "fullname", "password", "rank_id", "username" },
                values: new object[] { new Guid("d8377373-0670-406d-9072-faa5011b3980"), new Guid("39d7d847-49ef-4434-b8aa-d1cd88d2430d"), "admin@admin.com", true, null, "changeme", new Guid("4288aa01-036a-47e4-9db8-61e425ac2d43"), "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_device_calls_device_id",
                table: "device_calls",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_clipboard_logs_device_id",
                table: "device_clipboard_logs",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_contacts_device_id",
                table: "device_contacts",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_files_device_id",
                table: "device_files",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_installed_apps_device_id",
                table: "device_installed_apps",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_locations_device_id",
                table: "device_locations",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_notifications_device_id",
                table: "device_notifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_permissions_device_id",
                table: "device_permissions",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_sms_device_id",
                table: "device_sms",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_voice_records_device_id",
                table: "device_voice_records",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_wifi_device_id",
                table: "device_wifi",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_devices_user_id",
                table: "devices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_rank_roles_rank_id",
                table: "rank_roles",
                column: "rank_id");

            migrationBuilder.CreateIndex(
                name: "IX_rank_roles_role_name",
                table: "rank_roles",
                column: "role_name");

            migrationBuilder.CreateIndex(
                name: "IX_users_rank_id",
                table: "users",
                column: "rank_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_calls");

            migrationBuilder.DropTable(
                name: "device_clipboard_logs");

            migrationBuilder.DropTable(
                name: "device_contacts");

            migrationBuilder.DropTable(
                name: "device_files");

            migrationBuilder.DropTable(
                name: "device_installed_apps");

            migrationBuilder.DropTable(
                name: "device_locations");

            migrationBuilder.DropTable(
                name: "device_notifications");

            migrationBuilder.DropTable(
                name: "device_permissions");

            migrationBuilder.DropTable(
                name: "device_sms");

            migrationBuilder.DropTable(
                name: "device_voice_records");

            migrationBuilder.DropTable(
                name: "device_wifi");

            migrationBuilder.DropTable(
                name: "rank_roles");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "ranks");
        }
    }
}
