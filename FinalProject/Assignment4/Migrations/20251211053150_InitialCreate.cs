using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Assignment4.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exchange",
                columns: table => new
                {
                    exchangeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange", x => x.exchangeid);
                });

            migrationBuilder.CreateTable(
                name: "ratecurve",
                columns: table => new
                {
                    ratecurveid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    curvename = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    curvedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratecurve", x => x.ratecurveid);
                });

            migrationBuilder.CreateTable(
                name: "underlying",
                columns: table => new
                {
                    underlyingid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    assettype = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_underlying", x => x.underlyingid);
                });

            migrationBuilder.CreateTable(
                name: "market",
                columns: table => new
                {
                    marketid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exchangeid = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_market", x => x.marketid);
                    table.ForeignKey(
                        name: "FK_market_exchange_exchangeid",
                        column: x => x.exchangeid,
                        principalTable: "exchange",
                        principalColumn: "exchangeid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratepoint",
                columns: table => new
                {
                    ratepointid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: false),
                    tenor = table.Column<double>(type: "double precision", nullable: false),
                    rate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratepoint", x => x.ratepointid);
                    table.ForeignKey(
                        name: "FK_ratepoint_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asian_option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    strike = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false),
                    otyp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asian_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_asian_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_asian_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barrier_option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    barrierlevel = table.Column<double>(type: "double precision", nullable: false),
                    barriertype = table.Column<string>(type: "text", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    strike = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false),
                    otyp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barrier_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_barrier_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_barrier_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "digital_option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    payout_amount = table.Column<double>(type: "double precision", nullable: false),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    strike = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false),
                    otyp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_digital_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_digital_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_digital_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historicalprice",
                columns: table => new
                {
                    historicalpriceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    pricetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastprice = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historicalprice", x => x.historicalpriceid);
                    table.ForeignKey(
                        name: "FK_historicalprice_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lookback_option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    strike = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false),
                    otyp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lookback_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_lookback_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_lookback_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    strike = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false),
                    otyp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "range_option",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ratecurveid = table.Column<int>(type: "integer", nullable: true),
                    underlyingid = table.Column<int>(type: "integer", nullable: false),
                    vol = table.Column<double>(type: "double precision", nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    b = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_range_option", x => x.id);
                    table.ForeignKey(
                        name: "FK_range_option_ratecurve_ratecurveid",
                        column: x => x.ratecurveid,
                        principalTable: "ratecurve",
                        principalColumn: "ratecurveid");
                    table.ForeignKey(
                        name: "FK_range_option_underlying_underlyingid",
                        column: x => x.underlyingid,
                        principalTable: "underlying",
                        principalColumn: "underlyingid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asian_option_ratecurveid",
                table: "asian_option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_asian_option_underlyingid",
                table: "asian_option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_barrier_option_ratecurveid",
                table: "barrier_option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_barrier_option_underlyingid",
                table: "barrier_option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_digital_option_ratecurveid",
                table: "digital_option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_digital_option_underlyingid",
                table: "digital_option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_historicalprice_underlyingid",
                table: "historicalprice",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_lookback_option_ratecurveid",
                table: "lookback_option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_lookback_option_underlyingid",
                table: "lookback_option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_market_exchangeid",
                table: "market",
                column: "exchangeid");

            migrationBuilder.CreateIndex(
                name: "IX_option_ratecurveid",
                table: "option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_option_underlyingid",
                table: "option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_range_option_ratecurveid",
                table: "range_option",
                column: "ratecurveid");

            migrationBuilder.CreateIndex(
                name: "IX_range_option_underlyingid",
                table: "range_option",
                column: "underlyingid");

            migrationBuilder.CreateIndex(
                name: "IX_ratepoint_ratecurveid",
                table: "ratepoint",
                column: "ratecurveid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asian_option");

            migrationBuilder.DropTable(
                name: "barrier_option");

            migrationBuilder.DropTable(
                name: "digital_option");

            migrationBuilder.DropTable(
                name: "historicalprice");

            migrationBuilder.DropTable(
                name: "lookback_option");

            migrationBuilder.DropTable(
                name: "market");

            migrationBuilder.DropTable(
                name: "option");

            migrationBuilder.DropTable(
                name: "range_option");

            migrationBuilder.DropTable(
                name: "ratepoint");

            migrationBuilder.DropTable(
                name: "exchange");

            migrationBuilder.DropTable(
                name: "underlying");

            migrationBuilder.DropTable(
                name: "ratecurve");
        }
    }
}
