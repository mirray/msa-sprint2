using BookingServiceGrpc.Entities;

namespace BookingServiceGrpc.Queries.PromoCodeService;

public class Validate:IQuery<PromoCode>
{
    
    public required string Code { get; set; }
    public required string UserId { get; set; }
}