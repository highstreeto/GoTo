# GoTo - Getting from A to B!

This is a Alexa skill with a corresponding service and AWS function which allows for searching for public transport offers or rides provided by individuals. This was a project I did for a course about *Mobile and Ubiquitous Systems*.

Feel free to browse the code and *maybe* ge inspired by it.

The rest of the documentation is in German but the code should be readable enough to get an idea of what it does.

## License

MIT

## Problembeschreibung

Kombination einer Öffi-App und einer Mitfahrbörse in einen Alexa-Skill. Bei der Suche von z.B. Hagenberg <-> Linz werden nicht nur die Busverbindungen angezeigt, sondern auch von jenen Benützern, die nach Linz pendeln, sollten diese noch Platz haben. Der Alexa-Skill selbst, soll die Schnittstelle für jene sein, die eine Fahrt benötigen. Eine separate Weboberfläche dient zum Eintragen der Fahrten, damit der Alexa-Skill selbst nicht zu kompliziert wird. Die Fahrten haben einen Startzeitpunkt und eine fixe Anzahl von freien Plätzen und nach dem Startzeitpunkt ist die Fahrt nicht mehr wählbar im Alexa-Skill.

## Lösungsansatz

Eigener Cloud-gehosteter Alexa-Skill, der mit einem REST-Service kommuniziert. Dieses REST-Service ist zuständig für die Kommunikation mit den Öffi-Schnittstellen und die Verwaltung der Fahrten. Das REST-Service ist ebenfalls in AWS gehostet. Für das Verwalten der Fahrten in der Mitfahrbörse steht eine Angular-App zur Verfügung, die mit dem REST-Service kommuniziert.

## Erwartete Ergebnisse

* Alexa-Skill Abfrage für ÖBB Fahrten und Mitfahrten
* REST-Service für Verbindung zu Öffis und die Mitfahrbörse
  * Eigene Datenbank für die Börse
* Angular-App, für die Verwaltung der Fahrten

