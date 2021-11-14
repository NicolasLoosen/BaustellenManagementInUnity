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



### Die Datenbank
Bei unserer Datenbank handelt es sich Google's Firebase oder genauer, deren Realtime Datenbank. Sollen also die Fahrzeugdaten eines Fahrzeugobjekts visualisiert werden, so müssen diese in der Datenbank (sowie in dem Enum des [DataInterface](DataInterface.md) vorhanden sein. Diese Realtime Database kann [hier](https://console.firebase.google.com/u/0/project/baumanagement-9b929/database/baumanagement-9b929/data) vorgefunden werden. 

### Einfügen neuer Daten
Wichtig ist, dass neu eingefügte Daten der generellen Struktur, welche der Beanklasse/Serializable Klasse "ConstructionValues" oder allgemein der Datenbank entnommen werden können, folgen.  
     
Hier ein Beispiel:  
![Alter Laster Beispiel](https://i.imgur.com/ax3G6HN.png)   

Hierbei ist die Aufteilung wichtig. Also, dass "arbeitsfläche" eine Breite, Höhe, Länge und einen Radius hat. Das Selbe gilt dann auch für die Strukturen von "bezugspunkt", "fahrweg", "standfläche" und "typ". Das neu hinzugefügte Objekt hätte dann auch einen "bezugspunkt" mit X,Y und Z, einen "fahrweg" mit einer Breite usw.
    
Wie bereits erwähnt muss dann anschließend das Objekt dem Enum im DataInterface hinzugefügt werden. Das geht durch hinzufügen des Namens des Objekts. Dabei ist darauf zu achten, das der Namen im Enum genau gleich dem Namen der Datenbank ist. In diesem Fall würden wir dann "AlterLaster" dem Enum hinzufügen.
