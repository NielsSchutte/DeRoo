using System.Collections.Generic;

public class Vragen
{
    public string vraag_id { get; set; }
    public string vraag_text { get; set; }
    public string categorie_id { get; set; }
    public string extra_commentaar { get; set; }
    public string actie_ondernomen { get; set; }
    public string persoon { get; set; }
    public string datum_gereed { get; set; }
    public string answer { get; set; }
    public byte[] foto1 { get; set; }
    public byte[] foto2 { get; set; }
    public byte[] foto3 { get; set; }
}

public class Categorien
{
    public string categorie_id { get; set; }
    public string categorie_text { get; set; }
    public string formulier_id { get; set; }
}

public class Formulieren
{
    public string formulier_id { get; set; }
    public string formulier_naam { get; set; }
    public string location { get; set; }
    public string date { get; set; }
    public string user { get; set; }
}


public class Gebruiker
{
    public string gebruiker_id { get; set; }
    public string token { get; set; }
    public string gebruiker_naam { get; set; }
    public string gebruiker_wachtwoord { get; set; }
    public string gebruiker_email { get; set; }
}

public class Medewerker
{
    public string id { get; set; }
    public string medewerker_naam { get; set; }
    public string geboortedatum { get; set; }
    public string min_aantal_toolboxen { get; set; }
}

public class Toolbox
{
    public string toolbox_id { get; set; }
    public string toolbox_onderwerp { get; set; }
}


public class RootObject
{
    public List<Vragen> vragen { get; set; }
    public List<Categorien> categorien { get; set; }
    public List<Formulieren> formulieren { get; set; }
    public List<Gebruiker> gebruiker { get; set; }
    public List<Medewerker> medewerkers { get; set; }
    public List<Toolbox> toolbox { get; set; }
}