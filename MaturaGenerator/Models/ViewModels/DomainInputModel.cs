using MaturaGenerator.Models.DomainConstraints;

namespace MaturaGenerator.Models.ViewModels;

public class DomainInputModel
{
    public string ConstantName { get; set; } = string.Empty;
    public string Type { get; set; } = "int"; // "int" or "fraction"
    public int MinValue { get; set; } = 1;
    public int MaxValue { get; set; } = 10;
    public int MinNumerator { get; set; } = 1;
    public int MaxNumerator { get; set; } = 10;
    public int MinDenominator { get; set; } = 1;
    public int MaxDenominator { get; set; } = 10;
    
    
    public DomainConstraint ToDomainEntity() => Type switch
    {
        "fraction" => new FractionDomain
        {
            ConstantNames = ConstantName,
            MinNumerator = MinNumerator,
            MaxNumerator = MaxNumerator,
            MinDenominator = MinDenominator,
            MaxDenominator = MaxDenominator
        },
        _ => new IntDomain
        {
            ConstantNames = ConstantName,
            MinValue = MinValue,
            MaxValue = MaxValue
        }
    };

    public static DomainInputModel FromDomainEntity(DomainConstraint domain) => domain switch
    {
        FractionDomain f => new DomainInputModel
        {
            ConstantName = f.ConstantNames,
            Type = "fraction",
            MinNumerator = f.MinNumerator,
            MaxNumerator = f.MaxNumerator,
            MinDenominator = f.MinDenominator,
            MaxDenominator = f.MaxDenominator
        },
        IntDomain i => new DomainInputModel
        {
            ConstantName = i.ConstantNames,
            Type = "int",
            MinValue = i.MinValue,
            MaxValue = i.MaxValue
        },
        _ => new DomainInputModel { ConstantName = domain.ConstantNames }
    };
}
