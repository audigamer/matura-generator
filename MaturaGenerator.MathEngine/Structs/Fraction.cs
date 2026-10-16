namespace MaturaGenerator.Models.Structs;

public struct Fraction(int numerator, int denominator)
{
    public int Numerator { get; set; } = numerator;
    public int Denominator { get; set; } = denominator;

    public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";
}