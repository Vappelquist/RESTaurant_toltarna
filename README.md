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

User
----------------------------------

GET /api/Booking: 

Get all bookings in the database.

´´´Json
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
  ´´´

----------------------------------
