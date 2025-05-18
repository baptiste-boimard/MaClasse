CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "UserProfiles" (
    "Id" character varying(255) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Name" character varying(255) NOT NULL,
    "GivenName" character varying(255) NOT NULL,
    "FamilyName" character varying(255) NOT NULL,
    "Picture" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_UserProfiles" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250323142048_Initial', '9.0.2');

CREATE TABLE "SessionData" (
    "Token" text NOT NULL,
    "UserId" text NOT NULL,
    "Role" text NOT NULL,
    "Expiration" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_SessionData" PRIMARY KEY ("Token")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250328172930_Ajout de la table SessionData', '9.0.2');

ALTER TABLE "SessionData" DROP CONSTRAINT "PK_SessionData";

ALTER TABLE "SessionData" RENAME TO "SessionDatas";

ALTER TABLE "UserProfiles" ADD "Role" character varying(255) NOT NULL DEFAULT '';

ALTER TABLE "SessionDatas" ADD CONSTRAINT "PK_SessionDatas" PRIMARY KEY ("Token");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250328180321_Ajout de Role dans UserProfile', '9.0.2');

ALTER TABLE "UserProfiles" ADD "Zone" character varying(255) NOT NULL DEFAULT '';

ALTER TABLE "SessionDatas" ALTER COLUMN "Role" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250405193247_Ajout de Zone dans UserProfile', '9.0.2');

ALTER TABLE "UserProfiles" ADD "IdRole" character varying(255) NOT NULL DEFAULT '';

CREATE TABLE "Rattachments" (
    "IdGuid" uuid NOT NULL,
    "IdDirecteur" text NOT NULL,
    "IdProfesseur" text NOT NULL,
    CONSTRAINT "PK_Rattachments" PRIMARY KEY ("IdGuid")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250406153619_Ajout IdRole dans UserProfile', '9.0.2');

COMMIT;

