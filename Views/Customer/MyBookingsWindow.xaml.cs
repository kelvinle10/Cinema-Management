using CinemaManagementSystem.Models;
using CinemaManagementSystem.Views.Admin;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class MyBookingsWindow : Window
    {
        private readonly int customerId;

        public MyBookingsWindow(int customerId)
        {
            InitializeComponent();

            this.customerId = customerId;

            LoadBookings();
        }

        // Load bookings from database
        private void LoadBookings()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            string status =
                (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString()
                ?? "All Status";

            using (DBContext db = new DBContext())
            {
                var query = db.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .Include(b => b.Payment)
                    .Where(b => b.CustomerId == customerId);

                // Search by booking code or movie title
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(b =>
                        b.BookingCode.ToLower().Contains(keyword) ||
                        b.Showtime.Movie.Title.ToLower().Contains(keyword));
                }

                // Filter by booking status
                if (status != "All Status")
                {
                    query = query.Where(b => b.Status == status);
                }

                var bookings = query
                    .OrderByDescending(b => b.BookingTime)
                    .Select(b => new
                    {
                        b.BookingId,
                        b.BookingCode,
                        MovieTitle = b.Showtime.Movie.Title,
                        StartTime = b.Showtime.StartTime,
                        Seats = string.Join(", ",
                            b.Tickets.Select(t =>
                                t.Seat.SeatRow + t.Seat.SeatNumber)),
                        b.TotalAmount,
                        PaymentStatus = b.Payment != null
                            ? b.Payment.Status
                            : "N/A",
                        b.Status
                    })
                    .ToList();

                dgBookings.ItemsSource = bookings;
            }
        }

        // Search bookings
        private void txtSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            LoadBookings();
        }

        // Filter by status
        private void cbStatusFilter_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                LoadBookings();
            }
        }

        // Refresh
        private void btnRefresh_Click(
            object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cbStatusFilter.SelectedIndex = 0;
            LoadBookings();
        }

        // View booking details
        private void btnDetails_Click(
            object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            dynamic selectedBooking = button.DataContext;

            int bookingId = selectedBooking.BookingId;

            BookingDetailsWindow details =
                new BookingDetailsWindow(bookingId);

            details.Owner = this;
            details.ShowDialog();

            LoadBookings();
        }

        // Back to dashboard
        private void btnBack_Click(
            object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}