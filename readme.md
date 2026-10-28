# XP Level System

## MVP – Beskrivelse

Formålet med prosjektet er å lage et XP- og levelsystem som kan brukes for en spiller eller figur i et spill.

Spilleren starter på level 1 med 0 XP. Hver gang spilleren får XP, beregner systemet hvilket level spilleren skal ha. For hver 100 XP spilleren samler totalt, øker level med 1.

Eksempel:
- 0 XP = Level 1
- 100 XP = Level 2
- 200 XP = Level 3
- 350 XP = Level 4

## Objekter

Systemet inneholder en `Player`-klasse med følgende egenskaper:

- **Name:** Navnet på spilleren.
- **Experience:** Spillerens totale XP.
- **Level:** Spillerens nåværende nivå, beregnet ut fra XP.
- **UserId:** ID-en til brukeren som eier spilleren.

I tillegg har systemet en `User`-klasse for registrering og innlogging.

## Regler for XP og Level

- En ny spiller starter med 0 XP og level 1.
- Spilleren øker ett level for hver 100 XP totalt.
- Spilleren kan øke flere levels samtidig.
- XP kan aldri reduseres.
- Bare positive XP-verdier er gyldige.
- XP og level skal ikke kunne endres direkte utenfra `Player`-klassen.
- Endringer i XP skal gå gjennom metoden `AddExperience()`.

## MVP-flytdiagram

Flyten for den opprinnelige MVP-en er tegnet i Excalidraw:

![MVP-flytdiagram](Excalidraw/mvp.png)

Diagrammet viser hvordan en spiller opprettes, får XP, får beregnet level og kan hente sin oppdaterte spillerstatus.

## Teknologi

- C# og .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker og Docker Compose
- Traefik
- pgAdmin
- JWT Bearer Authentication
- BCrypt for hashing av passord
- xUnit for automatiserte tester
- Git og GitHub

## Uke 1 – TDD og REST API

Første del av prosjektet var å lage domenelogikken for XP og level, samt et REST API.

Jeg brukte TDD med tre hovedfaser:

1. **RED:** Skrive en test som feiler.
2. **GREEN:** Implementere funksjonaliteten slik at testen består.
3. **REFACTOR:** Forbedre koden uten å endre funksjonaliteten.

I den første versjonen ble spillerne lagret i minnet gjennom et fake repository.

Git-historikken inneholder separate commits fra arbeidet.

## Uke 2 – Docker og PostgreSQL

I uke 2 ble prosjektet utvidet med PostgreSQL og Entity Framework Core.

Jeg satte opp Docker Compose med følgende tjenester:

- **API:** Kjører ASP.NET Core-applikasjonen.
- **PostgreSQL:** Lagrer spillere og brukere.
- **pgAdmin:** Administrasjon av databasen.
- **Traefik:** Reverse proxy for API-et.
- **api-health:** Kontrollerer at API-et svarer på health-endepunktet.

Databaseskjemaet håndteres med EF Core-migrasjoner.

API-et kan nå lagre spillere permanent i PostgreSQL i stedet for bare i minnet.

## Uke 3 – JWT og brukere

I uke 3 ble prosjektet utvidet med registrering, innlogging og JWT-autentisering.

### Registrering og innlogging

- Brukere kan registrere seg med brukernavn og passord.
- Passord hashes med BCrypt før de lagres i databasen.
- Ved innlogging kontrolleres passordet med BCrypt.
- Ved vellykket innlogging returnerer API-et et JWT-token.
- JWT-tokenet inneholder brukerens ID.

### Beskyttelse av spillere

Spillerendepunktene krever gyldig JWT.

Hver spiller er knyttet til en bruker gjennom `UserId`.

En innlogget bruker kan opprette, hente og oppdatere sine egne spillere, men får ikke tilgang til spillere som tilhører andre brukere.

Forsøk på å hente eller endre en annen brukers spiller returnerer `404 Not Found`.

Forespørsler uten gyldig JWT returnerer `401 Unauthorized`.

## Konfigurasjon

Prosjektet bruker konfigurasjon for database og JWT.

Følgende JWT-innstillinger finnes i `appsettings.json`:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirationMinutes`

JWT-nøkkelen må være satt før API-et startes. Den skal ikke legges i Git.

For lokal utvikling kan nøkkelen settes med .NET User Secrets:

```powershell
dotnet user-secrets set "Jwt:Key" "DIN_EGEN_HEMMELIGE_NOKKEL" --project src/XpLevelSystem.Api
```

Bruk en tilfeldig generert nøkkel på minst 32 byte.

Ved kjøring med Docker Compose brukes en `.env`-fil i prosjektets rotmappe:

```dotenv
POSTGRES_PASSWORD=sett_et_passord
PGADMIN_PASSWORD=sett_et_passord
JWT_KEY=sett_en_lang_tilfeldig_jwt_nokkel
```

`.env` er lagt til i `.gitignore` og skal ikke pushes til GitHub.

## Kjøre prosjektet med Docker

Opprett `.env`-filen med nødvendige verdier.

Fra prosjektets rotmappe:

```powershell
docker compose up -d --build
```

API-et er tilgjengelig gjennom Traefik på:

`http://localhost:8081`

Health-endepunkt:

`http://localhost:8081/health`

pgAdmin:

`http://localhost:5050`

For å stoppe tjenestene:

```powershell
docker compose down
```

## Kjøre automatiserte tester

Fra prosjektets rotmappe:

```powershell
dotnet test
```

Testene dekker blant annet:

- Opprettelse av spillere.
- Beregning av XP og level.
- Validering av ugyldig XP.
- REST API-endepunkter.
- Krav om JWT for spillerendepunkter.
- At brukere ikke kan hente eller endre andre brukeres spillere.

Ved siste testkjøring bestod alle 17 testene.

## Manuell testing

API-et er også testet manuelt gjennom Docker med PowerShell og `Invoke-RestMethod`.

Jeg har kontrollert at:

- Registrering lagrer en ny bruker i PostgreSQL.
- Innlogging returnerer et JWT-token.
- En innlogget bruker kan opprette en spiller.
- XP kan legges til og level beregnes korrekt.
- Oppdaterte verdier blir lagret og kan hentes fra databasen.
- En annen bruker får `404` ved forsøk på å hente eller endre spilleren.

## API-endepunkter

| Metode | Endepunkt | Beskrivelse |
|---|---|---|
| POST | `/api/auth/register` | Registrer ny bruker |
| POST | `/api/auth/login` | Logg inn og få JWT |
| POST | `/api/players` | Opprett spiller |
| GET | `/api/players/{id}` | Hent egen spiller |
| POST | `/api/players/{id}/experience` | Legg til XP |
| GET | `/health` | Kontroller API-status |

Spillerendepunktene krever JWT i `Authorization`-headeren.

Den opprinnelige API-kontrakten finnes i [api.md](api.md).