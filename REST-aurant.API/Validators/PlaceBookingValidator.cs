using FluentValidation;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Validators
{
    public class PlaceBookingValidator : AbstractValidator<PlaceBookingRequest>
    {
        public PlaceBookingValidator()
        {
            RuleFor(x => x.FirstName)
                 .NotEmpty().WithMessage("FirstName is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("You must provide either an email or phone number.");

            RuleFor(x => x.BookingDate)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Booking date must be today or in the future.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .Must(t => TimeOnly.TryParse(t, out _))
                .WithMessage("Time must be entered in format HH:mm. For example 18:30")
                .Must(t =>
                {
                    if (!TimeOnly.TryParse(t, out var time)) return false;
                    return time >= TimeOnly.Parse("17:00") && time <= TimeOnly.Parse("22:00");
                })
                .WithMessage("Start time must be between 17:00 and 22:00.")
                .Must(t =>
                {
                    if (!TimeOnly.TryParse(t, out var time)) return false;
                    return time.Minute == 0 || time.Minute == 30;
                })
                .WithMessage("Start time must be on the hour or half hour. For example 18:00 or 18:30");
        }
    }
}
