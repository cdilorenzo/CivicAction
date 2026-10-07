namespace CivicAction.Domain.Participation;

/// <summary>A coarse place in Berlin: one of the 12 districts, or the whole city. An unknown place is a missing value.</summary>
public enum Locality
{
    CityWide = 1,
    Mitte = 2,
    FriedrichshainKreuzberg = 3,
    Pankow = 4,
    CharlottenburgWilmersdorf = 5,
    Spandau = 6,
    SteglitzZehlendorf = 7,
    TempelhofSchoeneberg = 8,
    Neukoelln = 9,
    TreptowKoepenick = 10,
    MarzahnHellersdorf = 11,
    Lichtenberg = 12,
    Reinickendorf = 13,
}
