# API-kontrakt – XP Level System

API-et lar en klient opprette spillere, hente spillerstatus og tildele XP. Spillere lagres midlertidig i minnet. Ingen database eller innlogging brukes i MVP-en.

## 1. Opprett spiller

**POST** `/api/players`

Request:
```json
{
  "name": "Martin"
}
```

Response – **201 Created**:
```json
{
  "id": 1,
  "name": "Martin",
  "experience": 0,
  "level": 1
}
```

Feil:
- **400 Bad Request** dersom navnet er tomt eller mangler.

## 2. Hent spiller

**GET** `/api/players/{id}`

Response – **200 OK**:
```json
{
  "id": 1,
  "name": "Martin",
  "experience": 0,
  "level": 1
}
```

Feil:
- **404 Not Found** dersom spilleren ikke finnes.

## 3. Tildel XP

**POST** `/api/players/{id}/experience`

Request:
```json
{
  "amount": 150
}
```

Response – **200 OK**:
```json
{
  "id": 1,
  "name": "Martin",
  "experience": 150,
  "level": 2
}
```

Feil:
- **400 Bad Request** dersom `amount` er 0 eller negativ.
- **404 Not Found** dersom spilleren ikke finnes.

## Forretningsregler

- Nye spillere starter med 0 XP og level 1.
- Hver 100 XP gir ett ekstra level.
- En spiller kan øke flere levels i én operasjon.
- XP kan ikke reduseres.
- Level beregnes automatisk fra samlet XP.

Formel: `Level = 1 + (Experience / 100)` med heltallsdivisjon.