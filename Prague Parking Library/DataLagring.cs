
using System.Text.Json;

namespace Prague_Parking_Library;

public class DataLagring
{
    private static readonly string Filnamn = "garagedata.json";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void SparaGarage(ParkeringsGarage garage)
    {
        var sparadeFordon = new List<SparatFordon>();
        var sparadeRegNr = new HashSet<string>();

        foreach (var ruta in garage.Rutor)
        {
            foreach (var fordon in ruta.ParkeradeFordon)
            {
                if (sparadeRegNr.Add(fordon.RegNr))
                {
                    sparadeFordon.Add(new SparatFordon
                    {
                        Fordonstyp = fordon.Fordonstyp,
                        RegNr = fordon.RegNr,
                        AnkomstTid = fordon.AnkomstTid,
                        Plats = ruta.Plats,
                        Storlek = fordon.Storlek,
                    });
                }
            }
        }

        string json = JsonSerializer.Serialize(sparadeFordon, Options);
        File.WriteAllText(Filnamn, json);
    }

    public static void LaddaGarage(ParkeringsGarage garage, AppConfig config)
    {
        if (!File.Exists(Filnamn))
        {
            return;
        }

        string json = File.ReadAllText(Filnamn);
        List<SparatFordon> sparadeFordon =
            JsonSerializer.Deserialize<List<SparatFordon>>(json, Options) ?? new List<SparatFordon>();

        foreach (var sparatFordon in sparadeFordon)
        {
            Fordon fordon;

            if (sparatFordon.Fordonstyp == "Bil")
            {
                fordon = new Bil(sparatFordon.RegNr, sparatFordon.Storlek);
            }
            else if (sparatFordon.Fordonstyp == "MC")
            {
                fordon = new MC(sparatFordon.RegNr, sparatFordon.Storlek);
            }
            else if (sparatFordon.Fordonstyp == "Buss")
            {
                fordon = new Buss(sparatFordon.RegNr, sparatFordon.Storlek);
            }
            else if (sparatFordon.Fordonstyp == "Cykel")
            {
                fordon = new Cykel(sparatFordon.RegNr, sparatFordon.Storlek);
            }
            else
            {
                throw new InvalidOperationException($"Okänd fordonstyp i sparfilen: {sparatFordon.Fordonstyp}");
            }

            fordon.AnkomstTid = sparatFordon.AnkomstTid;
            if (sparatFordon.Fordonstyp == "Buss")
            {
                int maxKapacitetPerRuta = config.GarageSettings.MaxKapacitetPerRuta;
                int antalRutor = sparatFordon.Storlek / maxKapacitetPerRuta;
                bool lyckades = true;

                for (int i = 0; i < antalRutor; i++)
                {
                    int aktuellPlats = sparatFordon.Plats + i;
                    if (aktuellPlats > garage.Rutor.Count || !garage.Rutor[aktuellPlats - 1].LäggTillFordon(fordon))
                    {
                        lyckades = false;
                        break;
                    }
                }

                if (!lyckades)
                {
                    throw new InvalidOperationException($"Kunde inte läsa in buss {sparatFordon.RegNr} på startplats {sparatFordon.Plats}.");
                }
            }
            else
            {
                if (!garage.LäggTillFordon(fordon, sparatFordon.Plats))
                {
                    throw new InvalidOperationException(
                        $"Kunde inte läsa in fordon {sparatFordon.RegNr} på plats {sparatFordon.Plats}.");
                }
            }
        }
    }

    public static void TestDataOmFilSaknas(ParkeringsGarage garage, AppConfig config)
    {
        if (File.Exists(Filnamn))
        {
            return;
        }

        garage.LäggTillFordon(new Bil("ABC123", 4), 1);
        garage.LäggTillFordon(new MC("QWE123", 2), 2);

        SparaGarage(garage);
    }

}
public class SparatFordon
{
    public string Fordonstyp { get; set; } = "";
    public string RegNr { get; set; } = "";
    public DateTime AnkomstTid { get; set; }
    public int Plats { get; set; }
    public int Storlek { get; set; }
}
   