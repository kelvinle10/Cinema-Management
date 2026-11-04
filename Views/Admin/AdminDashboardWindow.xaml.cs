using CinemaManagementSystem.Models;
using CinemaManagementSystem.Views.Auth;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class AdminDashboardWindow : Window
    {
        public AdminDashboardWindow()
        {
            InitializeComponent();
            LoadDashboardData();
        }


        private void LoadDashboardData()
        {
            using (DBContext db = new DBContext())
            {
                // Count total records
                int totalMovies = db.Movies.Count();
                int totalCustomers = db.Customers.Count();
                int totalBookings = db.Bookings.Count();
                int totalRooms = db.Rooms.Count();

                // Calculate total revenue
                decimal totalRevenue = db.Payments
                    .Where(p => p.Status == "Paid")
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Count bookings by status
                int pendingBookings = db.Bookings
                    .Count(b => b.Status == "Pending");

                int confirmedBookings = db.Bookings
                    .Count(b => b.Status == "Confirmed");

                int cancelledBookings = db.Bookings
                    .Count(b => b.Status == "Cancelled");

                int expiredBookings = db.Bookings
                    .Count(b => b.Status == "Expired");

                // Display general statistics
                txtTotalMovies.Text = totalMovies.ToString();
                txtTotalCustomers.Text = totalCustomers.ToString();
                txtTotalBookings.Text = totalBookings.ToString();
                txtTotalRooms.Text = totalRooms.ToString();
                txtTotalRevenue.Text = totalRevenue.ToString("N0") + " VND";

                // Display booking statistics
                txtPendingBookings.Text = pendingBookings.ToString();
                txtConfirmedBookings.Text = confirmedBookings.ToString();
                txtCancelledBookings.Text = cancelledBookings.ToString();
                txtExpiredBookings.Text = expiredBookings.ToString();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
        private void btnMovies_Click(object sender, RoutedEventArgs e)
        {
            MoviesWindow moviesWindow = new MoviesWindow();
            moviesWindow.Show();
            this.Close();
        }
        private void btnRooms_Click(object sender, RoutedEventArgs e)
        {
            RoomsWindow roomsWindow = new RoomsWindow();
            roomsWindow.Show();
            this.Close();
        }

        private void btnShowtimes_Click(object sender, RoutedEventArgs e)
        {
            ShowtimesWindow showtimesWindow = new ShowtimesWindow();
            showtimesWindow.Show();
            this.Close();
        }
        private void btnBookings_Click(object sender, RoutedEventArgs e)
        {
            BookingsWindow bookingsWindow = new BookingsWindow();
            bookingsWindow.Show();
            this.Close();
        }

        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            CustomersWindow customersWindow = new CustomersWindow();
            customersWindow.Show();
            this.Close();
        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow();
            reportsWindow.Show();
            this.Close();
        }
    }
}