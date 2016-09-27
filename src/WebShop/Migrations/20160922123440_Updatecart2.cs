using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebShop.Migrations
{
    public partial class Updatecart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductTranslations_TranslationProductId_TranslationLanguage",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_TranslationProductId_TranslationLanguage",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TranslationLanguage",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TranslationProductId",
                table: "Carts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslationLanguage",
                table: "Carts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TranslationProductId",
                table: "Carts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_TranslationProductId_TranslationLanguage",
                table: "Carts",
                columns: new[] { "TranslationProductId", "TranslationLanguage" });

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductTranslations_TranslationProductId_TranslationLanguage",
                table: "Carts",
                columns: new[] { "TranslationProductId", "TranslationLanguage" },
                principalTable: "ProductTranslations",
                principalColumns: new[] { "ProductId", "Language" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
