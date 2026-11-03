
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class ShowtimesWindow : Window
    {
        public ShowtimesWindow()
        {
            InitializeComponent();
            LoadShowtimes();
        }

        private void LoadShowtimes()
        {
            using (DBContext db = new DBContext())
            {
                dgShowtimes.ItemsSource = db.Showtimes
                    .Select(s => new
                    {
                        s.ShowtimeId,
                        MovieTitle = s.Movie.Title,
                        RoomName = s.Room.RoomName,
                        s.StartTime,
                        s.EndTime,
                        s.TicketPrice,
                        s.Status
                    })
                    .ToList();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            using (DBContext db = new DBContext())
            {
                dgShowtimes.ItemsSource = db.Showtimes
                    .Where(s => s.Movie.Title.ToLower().Contains(keyword))
                    .Select(s => new
                    {
                        s.ShowtimeId,
                        MovieTitle = s.Movie.Title,
                        RoomName = s.Room.RoomName,
                        s.StartTime,
                        s.EndTime,
                        s.TicketPrice,
                        s.Status
                    })
                    .ToList();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard = new AdminDashboardWindow();
            dashboard.Show();
            this.Close();
        }


        private void btnAddShowtime_Click(object sender, RoutedEventArgs e)
        {
            AddShowtimeWindow addWindow = new AddShowtimeWindow();

            if (addWindow.ShowDialog() == true)
            {
                LoadShowtimes();
            }
        }


        private void btnEditShowtime_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int showtimeId = (int)button.DataContext.GetType()
                .GetProperty("ShowtimeId")!
                .GetValue(button.DataContext)!;

            EditShowtimeWindow editWindow = new EditShowtimeWindow(showtimeId);

            if (editWindow.ShowDialog() == true)
            {
                LoadShowtimes();
            }
        }

        private void btnDeleteShowtime_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int showtimeId = (int)button.DataContext.GetType()
                .GetProperty("ShowtimeId")!
                .GetValue(button.DataContext)!;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this showtime?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            using (DBContext db = new DBContext())
            {
                Showtime? showtime = db.Showtimes
                    .FirstOrDefault(s => s.ShowtimeId == showtimeId);

                if (showtime == null)
                {
                    MessageBox.Show("Showtime not found!");
                    return;
                }

                bool hasBookings = db.Bookings
                    .Any(b => b.ShowtimeId == showtimeId);

                bool hasTickets = db.Tickets
                    .Any(t => t.ShowtimeId == showtimeId);

                if (hasBookings || hasTickets)
                {
                    MessageBox.Show(
                        "Cannot delete this showtime because it has related bookings or tickets!",
                        "Delete Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                db.Showtimes.Remove(showtime);
                db.SaveChanges();
            }

            MessageBox.Show("Showtime deleted successfully!");
            LoadShowtimes();
        }

    }
}
