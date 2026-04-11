namespace BookingServiceGrpc.Queries.UserService;

public class IsUserActive:IQuery<bool>
{
    public required string UserId { get; set; }
}
