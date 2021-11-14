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



### Erklärung
Das DataInterface (zu finden in Scripts/Database/DataInterface) dient als Schnittstelle zu den Datenbankanfragen.
Diese Daten sind Fahrzeugdaten der Objekte im Projekt, welche hauptsächlich Fahrzeuge wie z. B. Laster, Kräne etc. sind.
Dieses Interface bietet eine Methode namens "RequestConstructionVehicleAsync(ConstructionVehcileName vehicle)" welche einen generischen Task des Types ConstructionValues zurückliefert.
Wie dieses zu nutzen ist, steht auch ausführlich in der Dokumentation des Interfaces. Der Vollständigkeit halber möchten wir die Nutzung hier nochmal erläutern.   
### Nutzung
Hier ein Beispiel zur Nutzung des Interface:   

``` C#
Task<ConstructionVehicle> container_PausenRaumTask = RequestConstructionVehicleAsync(Container_Pausenraum);
ConstructionValues container_Pausenraum;   

if(container_PausenRaumTask.isFinished)    
{   
    container_Pausenraum = container_PausenRaumTask.Result;   
}   
```
Um die Daten aus der Datenbank für ein spezielles Objekt (Hier im Beispiel Container_Pausenraum) zu erhalten muss die Methode "RequestConstructionVehicleAsync" aus dem Interface gecalled werden.
Diese bekommt ein Objekt aus dem im selben Interface vorhanden Enum ConstructionVehicleNames. Die Objekte im Enum repräsentieren die Objekte aus der Datenbank.
Der Call für den Container_Pausenraum wäre also folgender.   
``` C#
Task<ConstructionVehicle> container_PausenRaumTask = RequestConstructionVehicleAsync(Container_Pausenraum);
```
bzw.
``` C#
Task<ConstructionVehicle> container_PausenRaumTask = DataInterface.RequestConstructionVehicleAsync(DataInterface.ConstructionVehicleNames.Container_Pausenraum);
```   
Die Methode liefert einen Task zurück welcher asynchron im Hintergrund des Programmes weiter läuft.
Die Werte des Pausenraumes ("länge", "breite", "höhe" etc.) sind erst nach Fertigstellung des Tasks verfügbar.
Ein Task kann über .isFinished nun überprüft werden, ob die Anfrage vollendet wurde.
Also muss vor dem Benutzen container_PausenRaumTask.isFinished true sein! Andernfalls läuft die Request nämlich noch.
Wenn .isFinisehd true ist, dann kann die Value des Tasks über .Result abgerufen werden. Dies ist dann kein Task mehr, sondern ein Objekt vom Typ ConstructionValues.
also in unserem Beispiel würde, dass dann wie folgt aussehen.   
``` C#
ConstructionValues container_Pausenraum = container_PausenRaumTask.Result;
```   
Wie in der Klasse ConstructionValues zu sehen ist, ist diese Klasse eine einfache Bean Klasse welche alle möglichen Getter und Setter für alle Werte zu Verfügung stellt.
Nun kann die Variable container_Pausenraum ganz normal genutzt werden, um auf alle Werte für den container_Pausenraum zuzugreifen.

### Logik/Implementierung
Die eigentliche Logik hinter den Requests befindet sich in ValueRetriever.
In der Dokumentation lässt sich nachvollziehen wie diese funktioniert.
Außerdem haben wir versucht, durch das Verwenden von vielen Methoden, möglichst viel Logik voneinander zu trennen. Dies soll mögliche Änderungen (falls notwendig) vereinfachen.
Sollten sich zum Beispiel Daten wie Login verändern, so sind diese in ValueRetriever in einer Unterklasse namens UserData zu finden, damit diese möglichst einfach und schnell abänderbar sind.

### Erweiterung
* Neue Objekte hinzufügen:   
  Beim Anfragen an die [Datenbank](Datenbank.md) wird das gewünschte Objekt über den exakten namen angefragt. 
  Daher muss beim Einfügen neuer Objekte in die Datenbank das neue Objekt auch 1-zu-1 mit gleichem Namen (Case Sensitive) auch in das Enum 
  "ConstructinoVehicleNames" eingefügt werden, welches sich in DataInterface befindet.   

* Objekte abändern:  
  Sollten die Objekte in der [Datenbank](Datenbank.md) umstrukturiert werden (bspw. durch das Hinzufügen oder Entfernen eines Attributes), so muss dies auch in der 
  repräsentativen Bean Klasse angepasst werden.
  Beispiel: Soll der Fahrweg ein neues Attribut erhalten wie z.B. eine Länge, so muss dieses Attribut in der Bean Klasse "ConstructionValues" hinzugefügt 
  werden um die neue Struktur zu definieren.
