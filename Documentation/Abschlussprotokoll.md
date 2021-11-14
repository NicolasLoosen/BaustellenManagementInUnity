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


Die zu Beginn der Projektarbeit definierten Ziele konnten nahezu alle umgesetzt werden. Stand vom 31.03.2020 kann die Anwendung dazu genutzt werden, die Einrichtung einer Baustelle über mehrere Abschnitte verteilt zu simulieren. 
Die Features der Anwendung umfassen:
* Platzieren von Objekten verschiedenster Art
* Manipulation der Position, Rotation und Größe von Objekten
* Darstellung einer Baustelle in verschiedenen Bauphasen
* Import von FBX-Modellen zur Laufzeit
* Polier-Modus, in dem die Baustelle zu Fuß erkundet werden kann
* Speichern und Laden von Baustellen

Lediglich der Virtual-Reality Modus konnte nicht vollständig umgesetzt werden, da es durch die COVID-19 bedingte Schließung der HTW nicht mehr möglich war, vor Ort mit der Oculus Rift zu arbeiten. Um der Anforderung jedoch wenigstens konzeptionell nachzukommen, wurde eine Variante der Anwendung erstellt, die mit einer VR-Brille funktionieren sollte, wenn die notwendigen Einstellungen im Unity-Editor gesetzt werden. Zum Zeitpunkt der Projektfertigstellung ist VR-Modus deaktiviert, um die Betrachter-Anwendung auch ohne spezielle Hardware testen zu können. Weitere Informationen dazu stehen im Wiki unter [Betrachter Anwendung](Betrachter-Anwendung.md).

## Update 2020/2021
### Erfüllte Ziele
* Grid-System
* Bewegbare/Editierbare 3D-Objects 
* CAD/IFC-Import
* UI-Design/Implementierung
* Montageablauf/Legende
* Rightclick-Menü auf allen Objekten
* Verbesserte User-Experience
* Undo/Redo
* Hinzufügen zusätzlicher Objekte
* Vorschaubilder für Objekte


## Future ToDos
* VR-Mode aktivieren und testen
* Datenbankanbindung Testen
* Löschen einzelner Bauphasen
* 