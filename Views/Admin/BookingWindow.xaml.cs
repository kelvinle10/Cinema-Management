
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class BookingsWindow : Window
    {
        public BookingsWindow()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            string status = "All Status";

            if (cbStatusFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                status = selectedItem.Content.ToString()!;
            }

            using (DBContext db = new DBContext())
            {
                var query = db.Bookings.AsQueryable();

                // Search by booking code, customer name or movie title
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(b =>
                        b.BookingCode.ToLower().Contains(keyword) ||
                        b.Customer.Account.FullName.ToLower().Contains(keyword) ||
                        b.Showtime.Movie.Title.ToLower().Contains(keyword));
                }

                // Filter by booking status
                if (status != "All Status")
                {
                    query = query.Where(b => b.Status == status);
                }

                dgBookings.ItemsSource = query
                    .Select(b => new
                    {
                        b.BookingId,
                        b.BookingCode,
                        CustomerName = b.Customer.Account.FullName,
                        MovieTitle = b.Showtime.Movie.Title,
                        b.Showtime.StartTime,
                        b.TotalAmount,
                        PaymentStatus = b.Payment != null
                            ? b.Payment.Status
                            : "No Payment",
                        b.Status
                    })
                    .OrderByDescending(b => b.BookingId)
                    .ToList();
            }
        }

        private void txtSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            LoadBookings();
        }

        private void cbStatusFilter_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (dgBookings != null)
            {
                LoadBookings();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard = new AdminDashboardWindow();
            dashboard.Show();
            this.Close();
        }


        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int bookingId = (int)button.DataContext.GetType()
                .GetProperty("BookingId")!
                .GetValue(button.DataContext)!;

            BookingDetailsWindow detailsWindow =
                new BookingDetailsWindow(bookingId);

            detailsWindow.ShowDialog();
        }



        private void btnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int bookingId = (int)button.DataContext.GetType()
                .GetProperty("BookingId")!
                .GetValue(button.DataContext)!;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel this booking?",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            using (DBContext db = new DBContext())
            {
                Booking? booking = db.Bookings
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    MessageBox.Show("Booking not found!");
                    return;
                }

                if (booking.Status == "Cancelled" ||
                    booking.Status == "Expired")
                {
                    MessageBox.Show("This booking cannot be cancelled!");
                    return;
                }

                // Update Booking status
                booking.Status = "Cancelled";

                // Update related Tickets
                var tickets = db.Tickets
                    .Where(t => t.BookingId == bookingId)
                    .ToList();

                foreach (Ticket ticket in tickets)
                {
                    ticket.Status = "Cancelled";
                }

                // Update Payment status
                Payment? payment = db.Payments
                    .FirstOrDefault(p => p.BookingId == bookingId);

                if (payment != null && payment.Status == "Paid")
                {
                    payment.Status = "Refunded";
                }

                db.SaveChanges();
            }

            MessageBox.Show("Booking cancelled successfully!");

            LoadBookings();
        }

    }
}
