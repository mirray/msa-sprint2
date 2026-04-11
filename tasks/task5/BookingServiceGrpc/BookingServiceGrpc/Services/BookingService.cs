using System.Globalization;
using Booking;
using BookingServiceGrpc.CommandHandlers;
using BookingServiceGrpc.Commands;
using BookingServiceGrpc.Producers;
using BookingServiceGrpc.Queries;
using BookingServiceGrpc.Queries.HotelService;
using BookingServiceGrpc.Queries.PromoCodeService;
using BookingServiceGrpc.Queries.ReviewService;
using BookingServiceGrpc.Queries.UserService;
using BookingServiceGrpc.QueryHandlers;
using Grpc.Core;

namespace BookingServiceGrpc.Services;

public class BookingService(IRestQueryHandlerProvider queryHandlerProvider,IBookingQueryHandler bookingQueryHandler,IBookingCommandHandler bookingCommandHandler, IKafkaProducer kafkaProducer, ILogger<BookingService> logger) : Booking.BookingService.BookingServiceBase
{
   public override async Task<BookingResponse> CreateBooking(BookingRequest request, ServerCallContext context)
   {
      await ValidateUser(request.UserId, context.CancellationToken);
      await ValidateHotel(request.HotelId, context.CancellationToken);
      
      var basePrice = await ResolveBestPrice(request.UserId, context.CancellationToken);
      var discount = await ResolvePromoDiscount(request.PromoCode,request.UserId, context.CancellationToken);
      var finalPrice = basePrice - discount;
      
      logger.LogInformation("Final price calculated: base={basePrice}, discount={discount}, final={finalPrice}", basePrice, discount, finalPrice);
      var bookingId = await bookingCommandHandler.Handle(new CreateBooking
      {
         UserId = request.UserId,
         HotelId = request.HotelId,
         PromoCode = request.PromoCode,
         Discount = discount,
         FinalPrice = finalPrice
      }, context.CancellationToken);
      var booking = await bookingQueryHandler.Handle(new GetBookingById { Id = bookingId });
      if (booking == null)
      {
         throw new Exception("Booking not found");
      }
      await kafkaProducer.ProduceMessageBookingCreated(booking, context.CancellationToken);
      return Map(booking);
   } 

   public override async Task<BookingListResponse> ListBookings(BookingListRequest request, ServerCallContext context)
   {
      var cancellationToken = context.CancellationToken;
      var bookings = await bookingQueryHandler.Handle(new GetBookings{ UserId = request.UserId}, cancellationToken);
      var response = new BookingListResponse{Bookings = { bookings.Select(Map) }};
      return response;
   }

   private async Task<double> ResolveBestPrice(string userId, CancellationToken cancellationToken)
   {
      var statusOpt = await queryHandlerProvider.Handle(new GetUserStatus { UserId = userId }, cancellationToken);
      var isVip = string.Compare(statusOpt, "VIP", StringComparison.OrdinalIgnoreCase) == 0;
      double price;
      if (statusOpt == string.Empty)
      {
         price = 100.0;
         logger.LogDebug("User {userId} has unknown status, default base price {price}", userId, price);
      }
      else
      {
         price = isVip ? 100.0 : 80.0;
         logger.LogDebug("User {userId} has status '{statusOpt}', base price is {price}", userId, statusOpt, price);
      }
      return price;
   }

   private async Task<double> ResolvePromoDiscount(string promoCode, string userId, CancellationToken cancellationToken)
   {
      if (promoCode == string.Empty) return 0.0;
      try
      {
         var promo = await queryHandlerProvider.Handle(new Validate{ Code = promoCode, UserId = userId}, cancellationToken);
         var promoDiscount = promo.Discount ?? 0.0;
         logger.LogDebug("Promo code '{promoCode}' applied with discount {promoDiscount}", promoCode, promoDiscount);
         return promoDiscount;
      }
      catch (AggregateException _)
      {
         logger.LogInformation("Promo code '{promoCode}' is invalid or not applicable for user {userId}", promoCode, userId);
         return 0.0;
      }
      
   }

   private async Task ValidateUser(string userId, CancellationToken cancellationToken)
   {
      if (!await queryHandlerProvider.Handle(new IsUserActive { UserId = userId }, cancellationToken))
      {
         logger.LogWarning("User {userId} is inactive", userId);
         throw new ArgumentException("User is inactive");
      }

      if (await queryHandlerProvider.Handle(new IsUserBlacklisted { UserId = userId }, cancellationToken))
      {
         logger.LogWarning("User {userId} is blacklisted", userId);
         throw new ArgumentException("User is blacklisted");
      }
   }
   private async Task ValidateHotel(string hotelId, CancellationToken cancellationToken)
   {
      if (!await queryHandlerProvider.Handle(new IsHotelOperational { HotelId = hotelId }, cancellationToken))
      {
         logger.LogWarning("Hotel {hotelId} is not operational", hotelId);
         throw new ArgumentException("Hotel is not operational");
      }

      if (!await queryHandlerProvider.Handle(new IsTrustedHotel { HotelId = hotelId }, cancellationToken))
      {
         logger.LogWarning("Hotel {hotelId} is not trusted", hotelId);
         throw new ArgumentException("Hotel is not trusted");
      }

      if (await queryHandlerProvider.Handle(new IsHotelFullyBooked { HotelId = hotelId }, cancellationToken))
      {
         logger.LogWarning("Hotel {hotelId} is fully booked", hotelId);
         throw new ArgumentException("Hotel is fully booked");
      }
   }


   private BookingResponse Map(Dac.Booking booking)
   {
      var response = new BookingResponse
      {
         UserId = booking.UserId,
         HotelId = booking.HotelId,
         CreatedAt = booking.CreatedAt.ToString("yyyy-MM-ddT00:00:00Z"),
         DiscountPercent = booking.DiscountPercent ?? 0,
         Id = booking.Id.ToString(),
         Price = booking.Price ?? 0,
         PromoCode = booking.PromoCode
      };
      return response;
   }
}