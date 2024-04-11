using System.Text.Json.Serialization;

namespace RobotWorkers;

public class RobotWorker
{
    public static readonly List<string> Occupations =
    [
        "Mechanic",
        "Welder",
        "Painter",
        "Assembler",
        "Inspector",
        "Tester",
    ];

    public required string Name { get; set; }
    public required string Model { get; set; }
    public required string Manufacturer { get; set; }
    public required string SerialNumber { get; set; }
    public required string Occupation { get; set; }
    public decimal QualityRating { get; set; }
    public decimal EfficiencyRating { get; set; }

    [JsonInclude]
    public decimal PricePerHour =>
        Math.Round(20 + QualityRating * 0.4m + EfficiencyRating * 0.25m + _random.Next(1, 25));

    private readonly Random _random = new Random();
}
