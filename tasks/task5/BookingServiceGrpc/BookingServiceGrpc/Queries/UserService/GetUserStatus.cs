namespace BookingServiceGrpc.Queries.UserService;

public class GetUserStatus:IQuery<string>
{
    public required string UserId { get; set; }
}