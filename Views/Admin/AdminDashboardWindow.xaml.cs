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
                txtTotalMovies.Text = db.Movies.Count().ToString();
                txtTotalCustomers.Text = db.Customers.Count().ToString();
                txtTotalBookings.Text = db.Bookings.Count().ToString();
                txtTotalRooms.Text = db.Rooms.Count().ToString();

                decimal totalRevenue = db.Payments
                    .Where(p => p.Status == "Success")
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                txtTotalRevenue.Text = totalRevenue.ToString("N0") + " VND";
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
    }
}