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

Systemet inneholder en `Player`-klasse med tre egenskaper:

- **Name:** Navnet på spilleren.
- **Experience:** Spillerens totale XP.
- **Level:** Spillerens nåværende nivå, beregnet ut fra XP.

## Regler for XP og Level

- En ny spiller starter med 0 XP og level 1.
- Spilleren øker ett level for hver 100 XP totalt.
- Spilleren kan øke flere levels samtidig.
- XP kan aldri reduseres.
- Forsøk på å legge til negativ XP skal avvises.
- Level beregnes automatisk ut fra spillerens totale XP.
- Bare positive XP-verdier er gyldige. Verdier på 0 eller lavere skal avvises med en feilmelding.
- XP og level skal ikke kunne endres direkte utenfra `Player`-klassen. Endringer i XP skal gå gjennom metoden `AddExperience()`.