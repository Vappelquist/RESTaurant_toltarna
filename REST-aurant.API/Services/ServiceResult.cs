using Restaurant.API.Services.Enums;

namespace Restaurant.API.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public ErrorType? ErrorType { get; set; }

        public int BookingId { get; set; }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
    }

}
