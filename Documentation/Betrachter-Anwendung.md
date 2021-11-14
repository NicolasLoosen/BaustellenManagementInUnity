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



Beim Erfassen der Anforderungen an das Programm, wurde der Wunsch geäußert, die Anwendung VR-Kompatibel zu bauen. Während der Umsetzung des Prototyps hat sich gezeigt, dass es nicht direkt möglich ist eine Anwendung umzusetzen, die den konventionellen und den VR-Einsatz unterstützt. Dies liegt an den grundsätzlich verschiedenen Konzepten von Bedienoberfläche, Bewegung und Interaktion mit der Anwendung.  
Wir haben uns auf den Kompromiss geeinigt, dass wir eine Betrachter-Anwendung ausliefern, die mit einer VR-Brille umgehen kann. Die Besonderheiten dabei sind zum einen, dass Objekte auf einer Baustelle nicht verändert werden können, sondern, wie der Name schon sagt, nur betrachtet. Zum anderen besitzt diese Variante der Anwendung keine Bedienoberfläche. Sie wird komplett über Tasten oder Kontroller gesteuert.  
Der Code dieser Anwendung hat eine sehr hohe Übereinstimmung mit dem eigentlichen Baustellen-Manager. Daher wird diese Anwendung als eigener Branch geführt (`viewer_master`), der auf den `master` aufbaut. Dies soll das Übernehmen von Änderungen in der Codebasis erleichtern.  
Wie dieser Branch weitergepflegt werden kann und was dabei zu beachten ist, wird im Folgenden beschrieben.

## Allgemeine Hinweise zum Pflegen
- Anpassungen aus der Codebasis (`master`-Branch) müssen manuell übernommen werden. 
- Die spezifischen Anpassungen für den Betrachter sind möglichst klein zu halten, denn je weniger Änderungen vorhanden, desto geringer ist die Gefahr von Konflikten mit Änderungen in der Codebasis und das Updaten fällt leichter.
- Alle Anpassungen am besten mit einem Kommentar-Tag versehen und dazu schreiben, warum diese Anpassung gemacht wurde (Bspw.: `//VIEWER: removed request to fix null-Reference to UI`). Dadurch kann bei Konflikten einfach festgestellt werden, welche Änderung behalten werden müssen.
- Die Commits sollten sehr klein sein und wenn möglich nur Änderungen enthalten, die zusammenhängende Zeilen umfassen.
- Die Änderungen einer Szene sind alle auf einmal zu machen und in der Commit-Beschreibung so festzuhalten, dass sie Anhand dieser wiederholt werden können. (Grund: siehe nächster Punkt)
- Szenen von Unity lassen sich in den seltensten Fällen sinnvoll mergen. Deshalb sind bei Konflikten immer die Szenen aus dem `master` zu übernehmen und die Anpassungen im `viewer_master` erneut durchzuführen.

## Möglichkeiten Änderungen zu übernehmen
Es gibt verschiedene Möglichkeiten Änderungen aus dem `master` in den `viewer_master` übernehmen, jeweils mit Vor- und Nachteilen. Hier werden zwei Varianten vorgestellt, die beide auf die Funktionalität von Git aufbauen:
1. **merge**: Dies ist die einfache Variante Code nicht-destruktiv zu übernehmen, resultiert aber ein einer komplexen Git-History und erschwert damit das Finden spezifischer Anpassungen für den Betrachter.
2. **rebase**: Die aufwändigere und komplizierte Version des Updates, die bisher zum Einsatz kam. Sie bietet jedoch den Vorteil einer sehr einfachen und aufgeräumten Git-History, da immer die Wurzel des Branches verschoben wird. Dadurch sind spezifische Anpassungen leichter zu finden und ein Vergleich zwischen `master` und `viewer_master` kein Problem. Der Vorteil wird aber mit einem Risiko erkauft: Die Git-History wird rückwirkend nicht-wiederherstellbar verändert. Das führt zu Problemen mit lokalen Kopien des Branches, auf anderen Systemen.

### Merge
Das Mergen ist eine Standardoperation von Git und sollte daher beherrscht werden. Beim Mergen ist darauf zu achten, dass Anpassungen im `viewer_master` nicht überschrieben werden. Dabei sind die Kommentar-Tags hilfreich. 

### Rebase 
**Mit Vorsicht anwenden und andere Entwickler informieren!**
Noch ein paar Hinweise:
- Der Branch sollte eine einfache lineare History haben, also keine weiteren Branches oder durchgeführte Merges
- Die Commits sollten wie oben beschrieben strukturiert sein
- Commits mit Änderungen in der Szene sollten im Betreff des Commits einen Hinweis darauf enthalten

So geht's:
- `viewer_master` auschecken
- interaktiver rebase zum `master`
- Commits mit einer Änderungen der Szene werden nicht übernommen (drop)
- rebase fertigstellen
- Änderungen an Szenen sind erneut durchzuführen und zu committen 

Nach diesem Vorgang ist die Wurzel des Branches der neueste Commit des `master`s. Im Branch sind nur die spezifischen Anpassungen als Commits vorhanden.