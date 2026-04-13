namespace BookingServiceHistory.Dac;

public class Booking {
    
    public long Id { get; set; }
    public string? UserId { get; set; }

    public string? HotelId { get; set; }
    public string? PromoCode { get; set; }
    public double? DiscountPercent { get; set; }
    public double? Price { get; set; }
    public DateTime CreatedAt { get; set; }
}