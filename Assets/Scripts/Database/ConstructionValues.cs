using System;

/*
 * This class is a bean class, that displays the Databasestructure. When values are stored in or retrieved from the Database, create an object of this class
 * so the structure stays the same. 
 * It is also used for http requests. In that case C# will understand what information from the .json it needs to store.
 */
[Serializable]
public class ConstructionValues
{
    public string name { get; set; } //is an Auto-Property which is used, so that name doesnt get a seperate slot in the database

    public string typ;

    public Arbeitsflaeche arbeitsflaeche = new Arbeitsflaeche();
    public Bezugspunkt bezugspunkt = new Bezugspunkt();
    public Standflaeche standflaeche = new Standflaeche();
    public Fahrweg fahrweg = new Fahrweg();



    [Serializable]
    public class Arbeitsflaeche
    {
        public double Breite;
        public double Hoehe;
        public double Laenge;
        public double Radius;
        
    }

    [Serializable]
    public class Bezugspunkt
    {
        public double X;
        public double Y;
        public double Z;
    }

    [Serializable]
    public class Fahrweg 
    {
        public double Breite;
    }

    [Serializable]
    public class Standflaeche
    {
        public double Breite;
        public double Laenge;
        public double Radius;
    }

    public String toString() {
        String ausgabe = "";
        ausgabe += "Typ: " + typ + 
            "\nArbeitsflaeche: " + "\n\tBreite: " + arbeitsflaeche.Breite + "\n\tHoehe: " + arbeitsflaeche.Hoehe + "\n\tLaenge: " + arbeitsflaeche.Laenge + "\n\tRadius: " + arbeitsflaeche.Radius +
            "\nBezugspunkte: " + "\n\tX: " + bezugspunkt.X + "\n\tY: " + bezugspunkt.Y + "\n\tZ: " + bezugspunkt.Z + 
            "\nFahrweg: " + "\n\tBreite: " + fahrweg.Breite +
            "\nStandflaeche: " + "\n\tBreite: " + standflaeche.Breite + "\n\tLaenge: " + standflaeche.Laenge + "\n\tRadius: " + standflaeche.Radius + "\n";
       
        return ausgabe;
    }



}
