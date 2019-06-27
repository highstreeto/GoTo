---
title: GoTo
subtitle: Alexa-Skill für Suche von Öffi-Verbindungen und Mitfahrten
author: Martin Hochstrasser
date: 27.06.2019
lang: de-DE
slideNumber: true
---

# Problem

Mitfahrten nicht einfach zu finden

Keine einheitliche Suche mit Öffis

# Lösungsansatz

![Systemüberblick](GoTo-Overview.svg)

## Erläuterung

* Alexa-Skill: Frontend für die Suche
* Angular-App: Frontend für das Eintragen der Fahrten
* ASP.NET Core: Backend für das Speichern der Fahrten und Suchen nach Öffi-Verbindungen

# GoTo.Service

**Zentrale** Schnittstelle für Angular und den Alexa Skill

<p class="fragment">Konfigurierbare Liste von Zielorten mit ihren Geo-Koordinaten</p>

<p class="fragment">Speichert Mitfahrten und sucht nach Verbindungen mit öffentlichen Verkehrsmitteln</p>

<p class="fragment">Abfrage von Orten über Name oder Geo-Koordinaten</p>

<p class="fragment">Benützt [Alexa Skills SDK for .NET](https://github.com/timheuer/alexa-skills-dotnet)</p>

## REST

![Swagger-UI der Schnittselle](goto-service-swagger.png)

## Suche nach Fahrten

Orte werden fuzzy gesucht. Es reicht dann aber eine exakte Suche.

Fahrten-Suche: zuerst bei den angeboteten Fahrten dann bei den öffentliche Verkehrsmitteln.

Google Maps - Einbindung war angedacht, es ist aber eine Kreditkarte (keine Prepaid) notwendig.

## Abfrage von Orten per Name

::: incremental

1. Entspricht der Ort einem bekannten Ort? (Groß-/Kleinschreibung irrelevant) -> wenn ja, Match
2. Ist er in einem bekannten Ort enthalten?  -> wenn ja, Match
3. Berechnung der *Levenshtein*-Distanz für alle Orte
4. Sortieren nach Distanz und unter 2? -> wenn ja, Match
5. Liefere alle Orte sortiert nach der *Levenshtein*-Distanz

:::

## Abfrage von Orten per Geo-Koordinaten

::: incremental

1. Berechne Distanz zwischen aktuellen Standort und bekannten Standorten
2. Alle Orten weiter als 100 km entfernt, werden aussortiert
3. Bester Match nach Distanz wird herangezogen

:::

# GoTo.Lambda

**Schnittselle** für den Alexa-Skill

Kann auch über *Location Services* den aktuellen Standort bestimmen

Muss die Zeitzone des Benutzers abfragen, um richtige Anfragen stellen zu können

Bestes Ergebnis wird vorgelesen, die Anderen sind als *Card* sichtbar

## Intents

* TripSearchIntent
  * Slot `Source` (AMAZON.City)
  * Slot `Destination` (AMAZON.City)
* SpecifyLocationIntent
  * Slot `Location` (AMAZON.City)

## Fehlerbehandlung

Ziel oder Startort nicht erkannt: `SpecifyLocationIntent` mit dem fehlenden Ort

Für Ort, wird höchsten 3-mal nachgefragt. Bei Überschritten wird die Konversation zurückgesetzt (Eigener Counter als Session-Attribute).

# Demo

**Web-Oberfläche**

---

![[GoTo](http://goto.eu-west-1.elasticbeanstalk.com/) aufrufen](goto-client-welcome.png)

---

![Fahrt anlegen](goto-client-newtrip.png)

---

![Neue Fahrt bewundern](goto-client-newtrip-added.png)

# Demo

**Alexa-Skill**

---

[Fahrtensuche mit Geo-Location](goto-alexa-geo.mp4)

(leider ohne Ton, da Android das Aufnehmen vom internen Audio nicht zulässt)

---

[Fahrtensuche ohne Geo-Location](goto-alexa-nogeo.mp4)

# Vielen Dank für eure Aufmerksamkeit!