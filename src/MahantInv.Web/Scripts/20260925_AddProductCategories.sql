-- =============================================================================
-- Migration : Add Product <-> Category many-to-many relationship
-- Database  : SQLite (MahantInventory.db)
-- Date      : 2026-09-25
--
-- Usage     : sqlite3 MahantInventory.db < Scripts/20260925_AddProductCategories.sql
--
-- Idempotent: safe to run more than once (IF NOT EXISTS everywhere).
-- Fresh databases do not need this script; EnsureCreated builds the same
-- schema from the EF model in MIDbContext.
-- =============================================================================

PRAGMA foreign_keys = ON;


-- -----------------------------------------------------------------------------
-- Table: Categories
-- Name is unique case-insensitively ("Electronics" == "electronics").
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id"   INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT    NOT NULL COLLATE NOCASE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Categories_Name" ON "Categories" ("Name");

-- -----------------------------------------------------------------------------
-- Table: ProductCategories (join table)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ProductCategories" (
    "Id"         INTEGER NOT NULL CONSTRAINT "PK_ProductCategories" PRIMARY KEY AUTOINCREMENT,
    "ProductId"  INTEGER NOT NULL,
    "CategoryId" INTEGER NOT NULL,
    CONSTRAINT "FK_ProductCategories_Products_ProductId"
        FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProductCategories_Categories_CategoryId"
        FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE
);

-- Composite unique constraint: a product can be linked to a category only once.
-- Its leading column (ProductId) also serves product -> categories lookups,
-- so a separate single-column ProductId index is not needed.
CREATE UNIQUE INDEX IF NOT EXISTS "IX_ProductCategories_ProductId_CategoryId"
    ON "ProductCategories" ("ProductId", "CategoryId");

-- Category -> products lookups (and FK cascade checks on category delete).
CREATE INDEX IF NOT EXISTS "IX_ProductCategories_CategoryId"
    ON "ProductCategories" ("CategoryId");

