# Arkitektur-review – XP Level System

## Innledning

Jeg har gått gjennom arkitekturen i XP Level System for å finne ting som kan forbedres. Jeg har sett på hvordan controllerne, servicene og repository-klassene fungerer sammen.

Jeg har valgt tre svakheter som kan påvirke hvordan prosjektet fungerer og hvor enkelt det er å vedlikeholde.

## 1. Ulik oppførsel i repository-klassene

**Problem:**

Jeg har to implementasjoner av `IPlayerRepository`: `FakePlayerRepository` og `EfPlayerRepository`.

Før forbedringen erstattet FakePlayerRepository hele spillerobjektet ved Update(), mens EfPlayerRepository bare kopierte navnet. XP og level ble lagret fordi EF Core allerede sporet objektet.

XP og level blir likevel lagret i dagens løsning fordi Entity Framework Core sporer endringene på spillerobjektet.

**Konsekvens:**

Hvis jeg senere sender inn et nytt `Player`-objekt til `Update()`, kan XP og level bli feil eller ikke bli oppdatert.

Testene som bruker `FakePlayerRepository`, kan også gi et annet resultat enn når programmet bruker PostgreSQL.

**Mulig løsning:**

Jeg vurderte to løsninger: å kopiere alle verdiene eksplisitt eller å fortsette å bruke EF Core-sporing.

Jeg valgte EF Core-sporing og la til en kontroll med ReferenceEquals. Da blir det tydelig at Update() krever et spillerobjekt som allerede spores av samme DbContext.

Jeg har også lagt til tester med SQLite for å kontrollere at XP og level blir lagret riktig.

De to repository-implementasjonene fungerer fortsatt ikke helt likt. Dette er dokumentert i ADR-en.

**Prioritet: Høy**

Dette er det viktigste problemet jeg vil forbedre i oppgaven.

## 2. Databasemigrasjoner ved oppstart

**Problem:**

I `Program.cs` kjører API-et `Database.Migrate()` når det starter, dersom database er konfigurert.

Det gjør at API-oppstart og endringer i databasestrukturen er koblet sammen.

**Konsekvens:**

Hvis en migrasjon feiler, kan API-et få problemer med å starte. Dette kan bli mer utfordrende hvis jeg senere kjører flere instanser av API-et.

**Mulig løsning:**

Jeg kan flytte migrering til et eget steg før API-et starter, for eksempel gjennom en egen kommando ved utrulling.

Da kan migrering og oppstart håndteres hver for seg.

**Prioritet: Middels**

Det fungerer greit i prosjektet slik det er nå, men bør vurderes hvis API-et skal brukes i et større miljø.

## 3. AuthController har mange oppgaver

**Problem:**

`AuthController` håndterer registrering, innlogging, databaseoppslag, passordhashing og opprettelse av JWT-token.

Det betyr at mye av autentiseringslogikken ligger direkte i controlleren.

**Konsekvens:**

Controlleren kan bli vanskeligere å vedlikeholde hvis jeg legger til flere funksjoner senere.

Det blir også vanskeligere å teste autentiseringslogikken separat fra HTTP-endepunktene.

**Mulig løsning:**

Jeg kan flytte innloggings- og registreringslogikken til en egen service, for eksempel `AuthService`.

JWT-generering kan eventuelt flyttes til en egen `JwtService`.

Da kan controlleren hovedsakelig ta imot forespørsler og returnere svar.

**Prioritet: Middels**

Dette er ikke en feil som stopper programmet i dag, men en forbedring som kan gjøre koden lettere å videreutvikle.

## Oppsummering

Jeg har valgt å jobbe videre med problemet i `EfPlayerRepository.Update()`.

Grunnen er at jeg allerede har fått tilbakemelding på at de to repository-implementasjonene ikke oppfører seg likt. Jeg vil undersøke to mulige løsninger, dokumentere valget i en ADR og gjennomføre forbedringen i koden.

De to andre svakhetene blir dokumentert, men jeg skal ikke endre dem i denne oppgaven.
