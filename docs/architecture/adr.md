# Architecture Decision Record

## Problem

Jeg har to repository-klasser som implementerer `IPlayerRepository`, men `Update()` fungerer litt forskjellig i dem.

`FakePlayerRepository` erstatter spillerobjektet, mens `EfPlayerRepository` bare kopierer navnet og kaller `SaveChanges()`.

I dagens løsning blir XP og level lagret fordi EF Core sporer objektet som ble hentet fra databasen. Det er ikke tydelig når man leser `Update()`-metoden.

Jeg vil gjøre oppførselen enklere å forstå uten å endre reglene i `Player`-klassen.

## Alternatives

### Alternativ 1 – Kopiere alle verdier

Endre `EfPlayerRepository.Update()` slik at den kopierer navn, XP og level fra objektet som sendes inn.

**Fordeler:**
- Oppdateringen blir tydelig.
- Kan fungere med et nytt spillerobjekt som ikke spores fra før.
- Mindre avhengig av EF Core-sporing.

**Ulemper:**
- XP og level har private setters.
- Jeg må lage en trygg måte å kopiere verdiene på.
- Mer kode å vedlikeholde hvis Player-klassen får flere egenskaper.

**Kompleksitet:** Middels.

**Sikkerhet:** Må fortsatt kontrollere at spilleren tilhører riktig bruker. Direkte endring av XP og level bør ikke tillates uten validering.

**Vedlikehold:** Flere felter og metoder må holdes oppdatert.

### Alternativ 2 – Bruke EF Core-sporing

La `EfPlayerRepository.Update()` lagre endringene på spillerobjektet som EF Core allerede sporer.

Da trenger ikke metoden å kopiere navnet separat.

**Fordeler:**
- Mindre kode.
- Beholder private setters på XP og level.
- Passer med hvordan PlayerService bruker repository i dag.

**Ulemper:**
- Fungerer bare når objektet spores av riktig DbContext.
- Kan gi uventet oppførsel hvis et nytt Player-objekt sendes inn.
- FakePlayerRepository og EfPlayerRepository har fortsatt ulik håndtering av objekter.

**Kompleksitet:** Lav.

**Sikkerhet:** Eksisterende kontroll av UserId må beholdes. Endringen gir ikke nye tilgangsrettigheter.

**Vedlikehold:** Enkel løsning så lenge vi fortsetter å bruke EF Core-sporing på samme måte.

## Decision

Jeg velger alternativ 2, å bruke EF Core-sporing.

Jeg vil fjerne linjen som kopierer navnet i `EfPlayerRepository.Update()` og beholde `SaveChanges()`.

Jeg vil også legge til en test som kontrollerer at XP og level blir lagret riktig.

## Why?

Dette passer best med hvordan prosjektet fungerer i dag.

`PlayerService` henter først spilleren fra repository, endrer XP gjennom `AddExperience()` og kaller deretter `Update()`.

EF Core sporer allerede dette objektet. Derfor trenger jeg ikke kopiere verdiene på nytt.

Jeg ønsker også å beholde private setters, slik at XP og level ikke kan endres fritt.

## Consequences

`EfPlayerRepository.Update()` bruker nå EF Core-sporing i stedet for å kopiere verdier manuelt.

Jeg har lagt til `ReferenceEquals` for å kontrollere at objektet som sendes inn, er det samme som EF Core allerede sporer. Hvis det er et annet objekt, kaster metoden `InvalidOperationException`.

Dette gjør det tydeligere hvordan metoden skal brukes, men det betyr også at FakePlayerRepository og EfPlayerRepository fortsatt ikke fungerer helt likt.

Jeg har lagt til to tester med SQLite:
- En test som kontrollerer at XP og level blir lagret i databasen.
- En test som kontrollerer at et annet spillerobjekt blir avvist.

Alle 22 testene består etter endringen.

Hvis jeg senere vil støtte oppdatering med nye spillerobjekter, må jeg endre løsningen og vurdere ADR-en på nytt.
