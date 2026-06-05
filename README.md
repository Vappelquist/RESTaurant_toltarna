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
