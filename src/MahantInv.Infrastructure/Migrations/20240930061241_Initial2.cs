using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahantInv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDocuments_Orders_OrderId",
                table: "OrderDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                table: "OrderTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransactions_Parties_PartyId",
                table: "OrderTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_PartyCategories_CategoryId",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpiries_Orders_OrderId",
                table: "ProductExpiries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpiries_Products_ProductId",
                table: "ProductExpiries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStorages_Products_ProductId",
                table: "ProductStorages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStorages_Storages_StorageId",
                table: "ProductStorages");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ProductExpiries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReceivedQuantity",
                table: "Orders",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDocuments_Orders_OrderId",
                table: "OrderDocuments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                table: "OrderTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransactions_Parties_PartyId",
                table: "OrderTransactions",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_PartyCategories_CategoryId",
                table: "Parties",
                column: "CategoryId",
                principalTable: "PartyCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExpiries_Orders_OrderId",
                table: "ProductExpiries",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExpiries_Products_ProductId",
                table: "ProductExpiries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStorages_Products_ProductId",
                table: "ProductStorages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStorages_Storages_StorageId",
                table: "ProductStorages",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDocuments_Orders_OrderId",
                table: "OrderDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                table: "OrderTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransactions_Parties_PartyId",
                table: "OrderTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_PartyCategories_CategoryId",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpiries_Orders_OrderId",
                table: "ProductExpiries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpiries_Products_ProductId",
                table: "ProductExpiries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStorages_Products_ProductId",
                table: "ProductStorages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStorages_Storages_StorageId",
                table: "ProductStorages");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ProductExpiries");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDocuments_Orders_OrderId",
                table: "OrderDocuments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                table: "OrderTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransactions_Parties_PartyId",
                table: "OrderTransactions",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_PartyCategories_CategoryId",
                table: "Parties",
                column: "CategoryId",
                principalTable: "PartyCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExpiries_Orders_OrderId",
                table: "ProductExpiries",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExpiries_Products_ProductId",
                table: "ProductExpiries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStorages_Products_ProductId",
                table: "ProductStorages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStorages_Storages_StorageId",
                table: "ProductStorages",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id");
        }
    }
}
