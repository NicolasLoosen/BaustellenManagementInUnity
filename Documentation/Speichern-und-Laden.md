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


### Speichern
Um den Zustand einer Baustelle zu sichern wird der `Speichern`-Button im Menü in der Baustellenansicht betätigt. Dieser öffnet den Dateibrowser des Computers mit dessen Hilfe Sie die Datei an einen beliebigen Ort speichern können.

### Laden
Um eine zuvor gespeicherte Baustelle wiederherzustellen, muss im Hauptmenü `Laden` betätigt werden. Wie zuvor beim Speichern, wird auch hier der Dateibrowser geöffnet und ermöglicht die Auswahl eines Speicherstandes. Wurde eine Datei ausgewählt, springt der Baustellenmanager automatisch in die Baustellenansicht und stellt den Zustand aus der Datei her. Müssen komplexe Gebäude berechnet werden, kann es zu Verzögerungen kommen.

### Verarbeitung importierter Modelle
Beim Speichern wird der Zustand importierter Modelle natürlich auch gespeichert. Die IFC/Prefabs-Datei selbst, deren Modell den platzierten Objekten als Vorlage dient, wird jedoch nicht übernommen. 

Beim Laden eines Speicherstandes, wird versucht die Vorlagen anhand des Dateinamens zu finden. Ist eine Vorlage in der Liste mit Modellen nicht zu finden, wird das Objekt übersprungen. Ein entsprechender Hinweis wird angezeigt. 

Es ist also zu beachten, dass beim Weitergeben importierte Modelle auf dem Zielsystem auch vorhanden und importiert sein müssen, um eine Datei vollständig laden zu können.