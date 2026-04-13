using BookingServiceHistory.Commands;
using BookingServiceHistory.DbContexts;

namespace BookingServiceHistory.CommandHandlers;

public interface ICreateHistoryCommandHandler
{
    Task<long> Handle(CreateHistoryBooking command, CancellationToken cancellationToken);
}

public class CreateHistoryCommandHandler(BookingHistoryDbContext dbContext) : ICreateHistoryCommandHandler
{
    public async Task<long> Handle(CreateHistoryBooking command, CancellationToken cancellationToken)
    {
        return await dbContext.CreateHistoryBooking(command.UserId, command.HotelId, command.PromoCode, command.Discount,
            command.FinalPrice, command.CreateAt, cancellationToken);
    }
}