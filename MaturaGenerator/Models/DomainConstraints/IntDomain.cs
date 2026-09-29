namespace MaturaGenerator.Models.DomainConstraints;

public class IntDomain : DomainConstraint
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    
    public override object GenerateValue(Random rng)
        => rng.Next(MinValue, MaxValue);
}