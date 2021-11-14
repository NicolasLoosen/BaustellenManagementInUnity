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



Das Projekt bietet die Möglichkeit Fahrzeugdaten anzufragen und zu visualisieren

Mit Rechtsklick auf ein Fahrzeug wird ein Kontextmenü geöffnet (siehe "Objekte verändern").       
Falls das Fahrzeug in der Datenbank ist, können Daten angefragt werden. Das ganze ist hier veranschaulicht:  
 
![RechtsclickMenue](https://i.imgur.com/cqpc2wz.png)   

Wird "Daten anfragen" gedrückt so kann es einen kurzen Moment dauern bis das Programm die Daten unserer Datenbank erhalten hat. Sind diese angekommen sieht das Ganze wie folgt aus.   

![RechtsclickMenueAuswahl](https://i.imgur.com/gu1ZztI.png)   

Drücken wir nun wie in diesem Beispiel "Arbeitsfläche", so wird diese anhand der Daten in der Datenbank visualisiert und sieht so aus:   

![BeispielArbeitsfläche](https://i.imgur.com/tyLcbBP.png)   

Das Selbe gilt auch für "Fahrweg" und "Standfläche".