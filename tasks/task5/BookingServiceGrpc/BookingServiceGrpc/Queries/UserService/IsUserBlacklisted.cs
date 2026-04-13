namespace BookingServiceGrpc.Queries.UserService;

public class IsUserBlacklisted:IQuery<bool>
{
    public required string UserId { get; set; }
}