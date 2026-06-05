Database structure:

User 
- Is shared by Guest and Admin
- Stores name, email, password, phonenumber, allergies and note

Bookings
- stores AmountOfGuests, DateBooked, StartTime, EndTime, Status and GuestId to make
  connection between Booking and Guest 

Tables 
– Stores TableNumber and Seats

BookingTables 
- stores which tables a guest has booked with BookingsId and TablesTableId

- A guest can have many bookings (one-to-many)
- A booking can have many tables (one-to-many)
- A booking is always connected to both a guest and a table

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

GET /api/Booking/[id}

Get booking by id.

*Request*
Enter booking id, ex. "50"

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
----------------------------------

PUT /api/Booking/[id}/details

Edit booking details.

*Request*
Enter booking id, ex. "50"
```Json
{
  "firstName": "Hugo",
  "lastName": "Jansson",
  "bookingNotes": "I want a window seat."
}
```

*Response*
- 200 "Booking details successfully updated."

----------------------------------

DELETE /api/Booking/{id}

Delete booking.

*Request* 
Enter Booking Id, ex. "50"

*Response*
- 200 "Booking deleted."

----------------------------------

Guest
----------------------------------

GET api/Guest

Get all guests.

*Response*
```Json
{
    "id": 1,
    "bookingStatuses": [
      "Confirmed"
    ],
    "firstName": "Anna",
    "lastName": "Andersson",
    "email": "anna@mail.com",
    "phoneNumber": "070-1112233",
    "allergies": null,
    "note": null
  },
  ```

----------------------------------

POST api/Guest

Register new guest.

*Request* 
```Json
{
  "firstName": "Hugo",
  "lastName": "Jönsson",
  "email": "hugo_jonsson@example.com",
  "phoneNumber": "0701234569",
  "allergies": "Nuts",
  "note": "string",
  "password": "stringsts123"
}
  ```

*Response*
```Json
{
  "firstName": "Hugo",
  "lastName": "Jönsson",
  "phoneNumber": "0701234569",
  "allergies": "Nuts",
  "note": "string",
  "bookings": null,
  "id": 93,
  "email": "hugo_jonsson@example.com",
  "password": "stringsts123"
}
  ```

----------------------------------

PUT api/Guest/{id}

Modify guest.

*Request* 
Enter guest id, ex. "50".
```Json
{
  "firstName": "Hugo",
  "lastName": "Jönsson",
  "email": "hugo_jons@example.com",
  "phoneNumber": "07055522213",
  "allergies": "kiwi",
  "note": "string"
}
  ```

*Response*
```Json
{
  "firstName": "Hugo",
  "lastName": "Jönsson",
  "phoneNumber": "07055522213",
  "allergies": "kiwi",
  "note": "string",
  "bookings": null,
  "id": 50,
  "email": "hugo_jons@example.com",
  "password": null
}
  ```


