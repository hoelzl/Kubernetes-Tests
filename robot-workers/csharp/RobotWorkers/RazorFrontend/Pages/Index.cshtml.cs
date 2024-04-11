using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorFrontend.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        ImageUrl =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABaElEQVQ4jZ2TTUvDQBDGn3";
    }

    public string ImageUrl { get; set; }
    public string WorkerJson { get; set; }
    public RobotWorker Worker { get; set; }

    public async Task OnGetAsync()
    {
        await FetchDataAsync();
    }

    public async Task OnPostRefreshAsync()
    {
        await FetchDataAsync();
    }

    private async Task FetchDataAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();

        var descriptionResponse = await httpClient.GetAsync("http://robot-worker-discovery:8080/worker");
        WorkerJson = await descriptionResponse.Content.ReadAsStringAsync();
        Worker = JsonSerializer.Deserialize<RobotWorker>(WorkerJson) ?? new RobotWorker();

        var robohashResponse =
            await httpClient.GetAsync("http://robohash-internal:8081/robohash/" +
                                      ComputeHashString(WorkerJson, 32));
        var imageStream = await robohashResponse.Content.ReadAsStreamAsync();
        var imageBytes = await ToByteArrayAsync(imageStream);
        ImageUrl = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
    }

    private static async Task<byte[]> ToByteArrayAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public static string ComputeHashString(string input, int length)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return length >= hashString.Length ? hashString : hashString[..length];
    }
}

public class RobotWorker
{
    [JsonPropertyName("name")] public string Name { get; set; } = "Unknown";

    [JsonPropertyName("model")] public string Model { get; set; } = "Unknown";

    [JsonPropertyName("manufacturer")] public string Manufacturer { get; set; } = "Unknown";

    [JsonPropertyName("serialNumber")] public string SerialNumber { get; set; } = "Unknown";

    [JsonPropertyName("occupation")] public string Occupation { get; set; } = "Unknown";

    [JsonPropertyName("qualityRating")] public decimal QualityRating { get; set; } = 0;

    [JsonPropertyName("efficiencyRating")] public decimal EfficiencyRating { get; set; } = 0;

    [JsonPropertyName("pricePerHour")] public decimal PricePerHour { get; set; } = 0;
}
