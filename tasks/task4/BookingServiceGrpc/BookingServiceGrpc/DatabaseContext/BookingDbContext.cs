using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace BookingServiceGrpc.DatabaseContext;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Dac.Booking> Bookings { get; set; } = null!;

    public async Task<long> CreateBooking(string userId, string hotelId, string promoCode, double discount, double finalPrice, CancellationToken cancellationToken)
    {
        var booking = new Dac.Booking
        {
            UserId = userId,
            HotelId = hotelId,
            DiscountPercent = discount,
            PromoCode = promoCode,
            Price = finalPrice,
            CreatedAt = DateTime.Today,
        };
        await Bookings.AddAsync(booking, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return booking.Id;
    }

    public IQueryable<Dac.Booking> ListBookings(string userId) => Bookings.AsNoTracking().Where(b => b.UserId == userId);
    public IQueryable<Dac.Booking> AllBookings() => Bookings.AsNoTracking();
    
    public async Task<Dac.Booking?> GetBookingById(long id, CancellationToken cancellationToken ) => await Bookings.AsNoTracking().SingleOrDefaultAsync(b => b.Id == id, cancellationToken);
}