using CinemaManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class SeatSelectionWindow : Window
    {
        private readonly int showtimeId;
        private readonly int customerId;
        private decimal ticketPrice;

        private List<SeatItem> seats = new List<SeatItem>();
        private List<int> selectedSeatIds = new List<int>();

        public SeatSelectionWindow(int showtimeId, int customerId)
        {
            InitializeComponent();

            this.showtimeId = showtimeId;
            this.customerId = customerId;

            LoadSeats();
        }

        private void LoadSeats()
        {
            using (DBContext db = new DBContext())
            {
                var showtime = db.Showtimes
                    .Where(s => s.ShowtimeId == showtimeId)
                    .Select(s => new
                    {
                        s.StartTime,
                        s.TicketPrice,
                        MovieTitle = s.Movie.Title,
                        RoomName = s.Room.RoomName,
                        RoomId = s.RoomId
                    })
                    .FirstOrDefault();

                if (showtime == null)
                {
                    MessageBox.Show("Showtime not found!");
                    Close();
                    return;
                }

                ticketPrice = showtime.TicketPrice;

                txtShowtimeInfo.Text =
                    $"{showtime.MovieTitle} | {showtime.RoomName} | " +
                    $"{showtime.StartTime:dd/MM/yyyy HH:mm}";

                var bookedSeatIds = db.Tickets
                    .Where(t => t.ShowtimeId == showtimeId
                        && (t.Status == "Reserved" || t.Status == "Sold")
                        && (t.Booking.Status == "Pending"
                            || t.Booking.Status == "Confirmed"))
                    .Select(t => t.SeatId)
                    .ToList();

                seats = db.Seats
                    .Where(s => s.RoomId == showtime.RoomId && s.IsActive)
                    .OrderBy(s => s.SeatRow)
                    .ThenBy(s => s.SeatNumber)
                    .Select(s => new SeatItem
                    {
                        SeatId = s.SeatId,
                        SeatName = s.SeatRow + s.SeatNumber,
                        IsAvailable = true,
                        SeatColor = "#444455"
                    })
                    .ToList();

                foreach (SeatItem seat in seats)
                {
                    if (bookedSeatIds.Contains(seat.SeatId))
                    {
                        seat.IsAvailable = false;
                        seat.SeatColor = "#A94444";
                    }
                }

                icSeats.ItemsSource = seats;
            }
        }

        private void btnSeat_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            SeatItem seat = (SeatItem)button.DataContext;

            if (!seat.IsAvailable)
                return;

            seat.IsSelected = !seat.IsSelected;

            if (seat.IsSelected)
            {
                seat.SeatColor = "#D9A441";
                selectedSeatIds.Add(seat.SeatId);
            }
            else
            {
                seat.SeatColor = "#444455";
                selectedSeatIds.Remove(seat.SeatId);
            }

            // Refresh seat display
            icSeats.ItemsSource = null;
            icSeats.ItemsSource = seats;

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            int totalSeats = selectedSeatIds.Count;
            decimal total = totalSeats * ticketPrice;

            txtSelectedSeats.Text = $"Selected seats: {totalSeats}";
            txtTotalPrice.Text = $"Total: {total:N0} VND";
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSeatIds.Count == 0)
            {
                MessageBox.Show("Please select at least one seat!");
                return;
            }

            using (DBContext db = new DBContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                // Check showtime
                var showtime = db.Showtimes
                    .FirstOrDefault(s => s.ShowtimeId == showtimeId
                        && s.Status == "Scheduled"
                        && s.StartTime > DateTime.Now
                        && s.Room.IsActive);

                if (showtime == null)
                {
                    MessageBox.Show("This showtime is no longer available!");
                    LoadSeats();
                    return;
                }

                // Check selected seats again
                bool seatAlreadyBooked = db.Tickets.Any(t =>
                    t.ShowtimeId == showtimeId &&
                    selectedSeatIds.Contains(t.SeatId));

                if (seatAlreadyBooked)
                {
                    MessageBox.Show(
                        "Some selected seats are no longer available. Please select again!");

                    LoadSeats();
                    return;
                }

                // Check seats belong to this room and are active
                int validSeatCount = db.Seats.Count(s =>
                    selectedSeatIds.Contains(s.SeatId) &&
                    s.RoomId == showtime.RoomId &&
                    s.IsActive);

                if (validSeatCount != selectedSeatIds.Count)
                {
                    MessageBox.Show("Some selected seats are invalid!");
                    LoadSeats();
                    return;
                }

                decimal totalAmount = selectedSeatIds.Count * showtime.TicketPrice;

                // Create Booking
                Booking booking = new Booking
                {
                    BookingCode = "BK" + Guid.NewGuid().ToString("N").Substring(0, 12),
                    CustomerId = customerId,
                    ShowtimeId = showtimeId,
                    BookingTime = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Pending"
                };

                db.Bookings.Add(booking);
                db.SaveChanges();

                // Create Tickets
                foreach (int seatId in selectedSeatIds)
                {
                    Ticket ticket = new Ticket
                    {
                        TicketCode = "TK" + Guid.NewGuid().ToString("N").Substring(0, 16),
                        BookingId = booking.BookingId,
                        ShowtimeId = showtimeId,
                        SeatId = seatId,
                        Price = showtime.TicketPrice,
                        Status = "Reserved",
                        CreatedAt = DateTime.Now
                    };

                    db.Tickets.Add(ticket);
                }

                // Create Payment
                Payment payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = totalAmount,
                    Method = "Demo",
                    Status = "Pending",
                    TransactionCode = "TXN" + Guid.NewGuid().ToString("N"),
                    PaymentTime = null
                };

                db.Payments.Add(payment);

                db.SaveChanges();
                transaction.Commit();

                PaymentWindow paymentWindow = new PaymentWindow(booking.BookingId);
                paymentWindow.Owner = this;
                paymentWindow.ShowDialog();

                Close();

                Close();
            }
        }

        public class SeatItem
        {
            public int SeatId { get; set; }

            public string SeatName { get; set; } = "";

            public bool IsAvailable { get; set; }

            public bool IsSelected { get; set; }

            public string SeatColor { get; set; } = "";
        }
    }
}