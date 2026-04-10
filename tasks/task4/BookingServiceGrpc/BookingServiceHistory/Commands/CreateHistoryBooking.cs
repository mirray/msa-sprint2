namespace BookingServiceHistory.Commands;

public class CreateHistoryBooking
{
    public string? UserId { get; set; }
    public string? HotelId { get; set; }
    public string? PromoCode { get; set; }
    public double? Discount { get; set; }
    public double? FinalPrice { get; set; } 
    public DateTime? CreateAt { get; set; }
}