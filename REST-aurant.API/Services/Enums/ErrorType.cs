namespace Restaurant.API.Services.Enums
{
    public enum ErrorType
    {
        GuestNotFound,
        BookingNotFound,
        BadRequest,
        RequestMissing,
        InvalidInput,
        InvalidTime,
        DateInThePast,
        FullyBooked,
        AlreadyCanceled,
        AlreadyComplete,
        AlreadyConfirmed,
        AlreadyPending,
        ContactDetailsTaken,
        TableNotFound,
        TableAlreadyExists,
        TableHasActiceBookings
    }
}
