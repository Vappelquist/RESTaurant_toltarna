Database structure:

Users 
- Is shared by Guest and Admin
- Stores name, email, password, phonenumber, allergies and note
Bookings
- stores AmountOfGuests, DateBooked, StartTime, EndTime, Status and GuestId to make
  connection between Booking and Guest 
Tables – Stores TableNumber and Seats
BookingTables - stores which tables a guest has booked with BookingsId and TablesTableId
A guest can have many bookings (one-to-many)
A booking can have many tables (one-to-many)
A booking is always connected to both a guest and a table

API structure:

Booking
----------------------------------

GET /api/Booking: 

Get all bookings in the database.

*Response*
```Json
{
    "bookingId": 1,
    "guestName": "Anna Andersson",
    "amountOfGuests": 2,
    "status": "Confirmed",
    "dateBooked": "2026-05-08",
    "startDate": "2026-07-11",
    "startTime": "19:30:00",
    "endDate": "2026-07-11",
    "endTime": "21:30:00",
    "bookingNotes": "Dejt-kväll",
    "tableNumbers": [
      1
    ]
  }
  ```

----------------------------------

POST /api/Booking

Place a new booking.

*Request*
```Json
{
  "firstName": "Hugo",
  "lastName": "Jansson",
  "bookingNotes": "string",
  "email": "hugo_jansson@example.com",
  "phoneNumber": "0701234560",
  "amountOfGuests": 2,
  "bookingDate": "2026-08-05",
  "startTime": "19:00"
}
```

*Response*
```Json
{
  "message": "Thank you, your booking has been received!",
  "bookingId": 50
}
```

----------------------------------

Get /api/Booking/[id}

Get booking by id.

*Request*
50

*Response*
```Json
{
  "bookingId": 50,
  "guestName": "Hugo Jansson",
  "amountOfGuests": 2,
  "status": "Pending",
  "dateBooked": "2026-06-05",
  "startDate": "2026-08-05",
  "startTime": "19:00:00",
  "endDate": "2026-08-05",
  "endTime": "21:00:00",
  "bookingNotes": "string",
  "tableNumbers": [
    2
  ]
}
```
