namespace BookingServiceGrpc.Queries.HotelService;

public class IsHotelOperational:IQuery<bool>
{
    public required string HotelId { get; set; }
}