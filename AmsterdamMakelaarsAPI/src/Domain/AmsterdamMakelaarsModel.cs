namespace Domain;

public class AmsterdamMakelaarsModel
{
    public List<Object> Objects { get; set; }
    public Paging Paging { get; set; }
    public int TotaalAantalObjecten { get; set; }
}

public class Object
{
    public long MakelaarId { get; set; }
    public string MakelaarNaam { get; set; }
    public string Woonplaats { get; set; }
    public string VerkoopStatus { get; set; }
}

public class Paging
{
    public int AantalPaginas { get; set; } //Total page
    public int HuidigePagina { get; set; } //Current page
}