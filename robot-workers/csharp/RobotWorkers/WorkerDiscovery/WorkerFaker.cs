using Bogus;

namespace RobotWorkers;

public static class WorkerFaker
{
    private static readonly List<string> Prefixes =
    [
        "Robo",
        "Mech",
        "Auto",
        "Cyber",
        "Neo",
        "Titan",
        "Fusion"
    ];

    private static readonly List<string> Postfixes =
    [
        "",
        " Pro",
        " Plus",
        " X",
        " Max",
        "Bot",
        "Unit",
        "Machine",
        "Droid",
        "Automaton",
        "Cyborg"
    ];

    private static readonly Faker<RobotWorker> Faker = new Faker<RobotWorker>().StrictMode(true)
        .RuleFor(w => w.Name, f => f.Name.FullName())
        .RuleFor(w => w.Model, f => f.PickRandom(Prefixes) + f.Hacker.Noun() + f.PickRandom(Postfixes))
        .RuleFor(w => w.Manufacturer, f => f.Company.CompanyName())
        .RuleFor(w => w.SerialNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(w => w.Occupation, f => f.PickRandom(RobotWorker.Occupations))
        .RuleFor(w => w.QualityRating, f => Math.Round(f.Random.Decimal(0, 100), 1))
        .RuleFor(w => w.EfficiencyRating, f => Math.Round(f.Random.Decimal(0, 100), 1));

    public static RobotWorker Generate() => Faker.Generate();
}
