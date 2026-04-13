namespace BookingServiceGrpc.Queries;

public class GetBookingById:IQuery<Dac.Booking?>
{
    public long Id { get; set; }
}