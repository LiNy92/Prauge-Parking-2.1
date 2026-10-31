
namespace Prague_Parking_Library;

public interface IParkeringsRuta
{
    int Plats { get; set; }
    int MaxKapacitet { get; set; }
    IReadOnlyList<IFordon> ParkeradeFordon { get; }
    bool SökLedigPlats(IFordon fordon);
    bool LäggTillFordon(IFordon fordon);
    bool TaBortFordon(string regNr);
    IFordon HittaFordonIRuta(string regNr);

}