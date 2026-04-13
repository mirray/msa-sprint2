namespace BookingServiceGrpc.Queries.ReviewService;

public class IsTrustedHotel:IQuery<bool>
{
    public string HotelId { get; set; }
}