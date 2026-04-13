using System.Text.Json.Serialization;

namespace BookingServiceGrpc.Entities;

public class PromoCode
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("discount")]
    public double? Discount { get; set; }
    [JsonPropertyName("vipOnly")]
    public bool? VipOnly {get; set; }
    [JsonPropertyName("expired")]
    public bool? Expired { get; set; }
    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil  { get; set; }
    [JsonPropertyName("description")]
    public string? Description  { get; set; }
}