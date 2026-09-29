using MaturaGenerator.Models.Structs;

namespace MaturaGenerator.Models.DomainConstraints;

public class FractionDomain : DomainConstraint
{
    public int MinNumerator { get; set; }
    public int MaxNumerator { get; set; }

    public int MinDenominator { get; set; }
    public int MaxDenominator { get; set; }
    
    public override object GenerateValue(Random rng)
        => new Fraction(
            rng.Next(MinNumerator, MaxNumerator),
            rng.Next(MinDenominator, MaxDenominator)
        );
}