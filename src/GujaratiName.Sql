BEGIN TRANSACTION;
ALTER TABLE "Products" ADD "GujaratiName" TEXT NOT NULL DEFAULT '';

INSERT INTO "MigrationHistory" ("MigrationId", "ProductVersion")
VALUES ('20250102163940_GujaratiName', '9.0.0');

COMMIT;

