CREATE TABLE "Country" (
	"Id" INT IDENTITY(1,1) NOT NULL,
	"ActiveCases" DECIMAL(10,0) NULL,
	"CountryName" VARCHAR(50) NOT NULL,
	"LastUpdate" DATE  NULL,
	"NewCases" DECIMAL(10,0)  NULL,
	"NewDeaths" DECIMAL(10,0)  NULL,
	"TotalCases" DECIMAL(10,0)  NULL,
	"TotalDeaths" DECIMAL(10,0)  NULL,
	"TotalRecovered" DECIMAL(10,0)  NULL,
	"IsDeleted" BIT NULL DEFAULT 0,
	
	PRIMARY KEY ("Id")
)
;


