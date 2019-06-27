---
title: GoTo
subtitle: Alexa-Skill für Suche von Öffi-Verbindungen und Mitfahrten
author: Martin Hochstrasser
date: 27.06.2019
lang: de-DE
---

# Problem

* Mitfahrten nicht einfach zu finden
* Keine einheitliche Suche mit Öffis

# Lösungsansatz

![Systemüberblick](GoTo-Overview.svg)

---

* Alexa-Skill: Frontend für die Suche
* Angular-App: Frontend für das Eintragen der Fahrten
* ASP.NET Core: Backend für das Speichern der Fahrten und Suchen nach Öffi-Verbindungen

# Ergebnisse



# Demo

**Web-Oberfläche**

---

![[GoTo](http://goto.eu-west-1.elasticbeanstalk.com/) aufrufen](goto-client-welcome.png)

---

![Fahrt anlegen](goto-client-newtrip.png)

---

![Neue Fahrt anlegen](goto-client-newtrip.png)

# Demo

**Alexa-Skill**

---

[Fahrtensuche mit Geo-Location](goto-alexa-geo.mp4)

(leider ohne Ton, da Android das Aufnehmen vom internen Audio nicht zulässt)

---

[Fahrtensuche ohne Geo-Location](goto-alexa-nogeo.mp4)