namespace BookingServiceGrpc.Queries.HotelService;

public class IsHotelFullyBooked:IQuery<bool>
{
    public required string HotelId { get; set; }
}