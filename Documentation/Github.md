# **Navigation**  

* [Home](Home.md)  
* [Projektbeschreibung](Projektbeschreibung.md)  <!-- Passend zur Readme (gleich?) -->
* [Abschlussprotokoll](Abschlussprotokoll.md)

**[Anwender](Anwender.md)**  <!-- Unterscheidung der Doku zwischen Anw und Dev -->
* [Getting Started](GettingStartedUser.md)
* [Hauptmenü](Hauptmenü.md)  
* [Baustelle](Baustelle.md)  
  * [Bewegen der Kamera](Bewegen-der-Kamera.md)
  * [Objekte platzieren](Objekte-platzieren.md)
  * [Objekte verändern](Objekte-verändern.md)
  * [Visualisieren](Fahrzeugdaten-visualisieren.md)
  * [Polieransicht](Polieransicht.md)
* [Speichern/Laden](Speichern-und-Laden.md)
* [Modelle importieren](Modelle-importieren.md)
* [Tastenkürzel](Tastenkürzel.md)
* [GitHub](Github.md)

***

**[Entwickler](Entwickler.md)**  
* [Getting Started](GettingStartedDev.md)
* [Prefabs hinzufügen](Prefabs-hinzufügen.md)
* [Overview](Overview.md)
* [Betrachter Anwendung](Betrachter-Anwendung.md)
* [Anwendung bauen](Anwendung-bauen.md)
* Visualisierung   
  * [Datenbank](Datenbank.md)
  * [DataInterface](DataInterface.md)


# Allgemeine Hinweise zum Pflegen
Das Projekt liegt unter https://github.com/htw-bauingenieurwesen/BaustellenManagementInUnity

Alle Kommandos sind in dem Verzeichnis des Projektes auszuführen.

## Git clone
Kopiert den Masterbranch in das aktuelle Verzeichnis.
- git clone https://github.com/htw-bauingenieurwesen/BaustellenManagementInUnity.git

## Git pull
aktualisiert das aktuelle Verzeichnis und mergt den remote branch in den local branch
- git pull 

## Git status
Überprüft den aktuellen Status des Projektes(ahead/behind of remote branch)
- git status

## Git commit
Committet die aktuellen Änderungen in einen Commit, dieser muss anschließend gepusht werden
- git commit -m "msg"

## Git push
Pusht die localen commits auf den remote branch
- git push
  
## Git merge
Merget einen branch in einen anderen branch
- git merge \<branchtomerge>

## Git checkout
Wechselt den Branch innerhalb des Repositorys
- git checkout <checkoutbranch>