
namespace Prague_Parking_Library;

public class ParkeringsGarage
{
    private readonly List<ParkeringsRuta> _rutor = new List<ParkeringsRuta>();
    public IReadOnlyList<IParkeringsRuta> Rutor => _rutor.AsReadOnly();
    public int KolumnPerRad { get; }
    public ParkeringsGarage(AppConfig config)
    {
        KolumnPerRad = config.GarageSettings.KolumnerPerRadUtskrift;
        int antalPlatser = config.GarageSettings.MaxAntalGaragePlatser;
        int maxKapacitetRuta = config.GarageSettings.MaxKapacitetPerRuta;

        for (int i = 1; i <= antalPlatser; i++)
        {
            _rutor.Add(new ParkeringsRuta(i, maxKapacitetRuta));
        }
    }

    public bool LäggTillFordon(IFordon fordon, int valdPlats)
    {
        if (valdPlats < 1 || valdPlats > _rutor.Count || SökaFordonsPlats(fordon.RegNr) != null)
        {
            return false;
        }

        return _rutor[valdPlats - 1].LäggTillFordon(fordon);
    }

    public Dictionary<string, int> BeräknaAntalFordon()
    {
        var totalen = new Dictionary<string, int>();
        foreach (var ruta in Rutor)
        {
            foreach (var fordon in ruta.ParkeradeFordon)
            {
                if (totalen.ContainsKey(fordon.Fordonstyp))
                {
                    totalen[fordon.Fordonstyp]++;
                }
                else
                {
                    totalen[fordon.Fordonstyp] = 1;
                }
            }
        }
        return totalen;
    }

    public IParkeringsRuta? HittaLedigPlats(IFordon fordon, int valdPlats = 0)
    {
        if (valdPlats < 0 || valdPlats > _rutor.Count)
        {
            return null;
        }

        if (valdPlats > 0)
        {
            ParkeringsRuta valdRuta = _rutor[valdPlats - 1];
            if (valdRuta.SökLedigPlats(fordon))
            {
                return valdRuta;
            }
            return null;
        }

        foreach (var ruta in Rutor)
        {
            if (ruta.SökLedigPlats(fordon))
            {
                return ruta;
            }
        }

        return null;
    }
    public IParkeringsRuta SökaFordonsPlats(string regNr)
    {
        foreach (var ruta in Rutor)
        {
            IFordon letarFordon = ruta.HittaFordonIRuta(regNr);

            if (letarFordon != null)
            {
                return ruta;
            }
        }
        return null;
    }
    public IFordon TaBortFordon(string regNr)
    {
        IParkeringsRuta ruta = SökaFordonsPlats(regNr);
        if (ruta == null)
        {
            return null;
        }
        IFordon fordonAttCheckaUt = ruta.HittaFordonIRuta(regNr);

        if (fordonAttCheckaUt != null)
        {
            ruta.TaBortFordon(regNr);
            return fordonAttCheckaUt;
        }
        return null;
    }

    public Flyttresultat FlyttaFordon(string regNr, int valdPlats)
    {
        if (valdPlats < 1 || valdPlats > _rutor.Count)
        {
            return Flyttresultat.OgiltigPlats;
        }

        IParkeringsRuta startRuta = SökaFordonsPlats(regNr);
        if (startRuta == null)
        {
            return Flyttresultat.FordonSaknas;
        }

        IFordon fordon = startRuta.HittaFordonIRuta(regNr);
        if (startRuta.Plats == valdPlats)
        {
            return Flyttresultat.SammaPlats;
        }

        IParkeringsRuta önskadRuta = HittaLedigPlats(fordon, valdPlats);
        if (önskadRuta == null || !önskadRuta.LäggTillFordon(fordon))
        {
            return Flyttresultat.UtrymmeSaknas;
        }

        startRuta.TaBortFordon(regNr);
        return Flyttresultat.Lyckats;
    }
    public List<IParkeringsRuta> HittaBussParkering(IFordon fordon, AppConfig config)
    {

        int kravAntalRutor = fordon.Storlek / config.GarageSettings.MaxKapacitetPerRuta;
        int bussParkering = config.GarageSettings.BussPlatser;


        for (int i = 0; i <= bussParkering - kravAntalRutor; i++)
        {
            bool allaLediga = true;
            List<IParkeringsRuta> möjligaRutor = new List<IParkeringsRuta>();

            for (int j = 0; j < kravAntalRutor; j++)
            {
                var ruta = _rutor[i + j];

                if (ruta.ParkeradeFordon.Count > 0) // Om rutan är upptagen - gå vidare
                {
                    allaLediga = false;
                    break;
                }

                möjligaRutor.Add(ruta); 
            }

            if (allaLediga)
            {
                return möjligaRutor;
            }
        }

        return null; // Ingen plass för buss
    }
    public void NyConfigFil(AppConfig nyFil)
    {
        int kolumnPerRad = nyFil.GarageSettings.KolumnerPerRadUtskrift;

        int nyaAntalet = nyFil.GarageSettings.MaxAntalGaragePlatser;

        while (_rutor.Count < nyaAntalet)
        {
            _rutor.Add(new ParkeringsRuta(_rutor.Count + 1, nyFil.GarageSettings.MaxKapacitetPerRuta));
        }

        while (_rutor.Count > nyaAntalet)
        {
            _rutor.RemoveAt(_rutor.Count - 1);
        }
    }
}