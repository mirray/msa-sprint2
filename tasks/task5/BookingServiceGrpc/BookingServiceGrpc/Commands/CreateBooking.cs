namespace BookingServiceGrpc.Commands;

public class CreateBooking:ICommand<long>
{
    public string UserId { get; set; }
    public string HotelId { get; set; }
    public string PromoCode { get; set; }
    public double Discount { get; set; }
    public double FinalPrice { get; set; } 
}