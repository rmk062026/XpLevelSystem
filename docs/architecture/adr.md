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

Koden i `EfPlayerRepository.Update()` blir enklere.

Løsningen vil fortsatt være avhengig av at spillerobjektet spores av samme DbContext.

Jeg må være oppmerksom på dette hvis jeg senere endrer hvordan PlayerService eller repository fungerer.

Jeg bør også ha en test mot EF Core, siden FakePlayerRepository ikke tester hvordan EF-sporing fungerer.
