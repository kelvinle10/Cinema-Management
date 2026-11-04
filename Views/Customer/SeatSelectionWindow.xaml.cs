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

            string selectedNames = string.Join(", ",
                seats.Where(s => selectedSeatIds.Contains(s.SeatId))
                     .Select(s => s.SeatName));

            MessageBox.Show(
                $"Selected seats: {selectedNames}\n" +
                $"Total: {(selectedSeatIds.Count * ticketPrice):N0} VND",
                "Seat Selection");
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