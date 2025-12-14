namespace MyWebAPI.Models
{
    public class Health
    {
        public string Status { get; set; } = "Healthy";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Version { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}