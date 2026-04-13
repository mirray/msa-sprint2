namespace BookingServiceGrpc.Queries;

public class GetBookings:IQuery<IEnumerable<Dac.Booking>>
{
    public string? UserId { get; set; }
}