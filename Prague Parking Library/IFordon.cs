

namespace Prague_Parking_Library;

public interface IFordon
{
    string Fordonstyp { get; set; }
    string RegNr { get; set; }
    int Storlek { get; set; }
    DateTime AnkomstTid { get; set; }
}
