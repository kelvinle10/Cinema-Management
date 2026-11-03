using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class RoomsWindow : Window
    {
        public RoomsWindow()
        {
            InitializeComponent();
            LoadRooms();
        }

        private void LoadRooms()
        {
            using (DBContext db = new DBContext())
            {
                var rooms = db.Rooms
                    .Select(r => new
                    {
                        r.RoomId,
                        r.RoomName,
                        r.Capacity,
                        TotalSeats = r.Seats.Count(),
                        r.IsActive
                    })
                    .ToList();

                dgRooms.ItemsSource = rooms;
            }
        }
        private void btnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            AddRoomWindow addRoomWindow = new AddRoomWindow();

            if (addRoomWindow.ShowDialog() == true)
            {
                LoadRooms();
            }
        }
        private void txtSearch_TextChanged(
    object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            using (DBContext db = new DBContext())
            {
                dgRooms.ItemsSource = db.Rooms
                    .Where(r => r.RoomName.ToLower().Contains(keyword))
                    .Select(r => new
                    {
                        r.RoomId,
                        r.RoomName,
                        r.Capacity,
                        TotalSeats = r.Seats.Count(),
                        r.IsActive
                    })
                    .ToList();
            }
        }

        private void btnEditRoom_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.DataContext == null)
            {
                return;
            }

            int roomId = (int)button.DataContext.GetType()
                .GetProperty("RoomId")!.GetValue(button.DataContext)!;

            EditRoomWindow editWindow = new EditRoomWindow(roomId);

            if (editWindow.ShowDialog() == true)
            {
                LoadRooms();
            }
        }

        private void btnDeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.DataContext == null)
            {
                return;
            }

            int roomId = (int)button.DataContext.GetType()
                .GetProperty("RoomId")!.GetValue(button.DataContext)!;

            using (DBContext db = new DBContext())
            {
                Room? room = db.Rooms.FirstOrDefault(
                    r => r.RoomId == roomId);

                if (room == null)
                {
                    MessageBox.Show("Room not found!");
                    return;
                }

                bool hasSeats = db.Seats.Any(s => s.RoomId == roomId);
                bool hasShowtimes = db.Showtimes.Any(s => s.RoomId == roomId);

                if (hasSeats || hasShowtimes)
                {
                    MessageBox.Show(
                        "Cannot delete this room because it has related seats or showtimes!",
                        "Delete Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete '{room.RoomName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    db.Rooms.Remove(room);
                    db.SaveChanges();

                    MessageBox.Show("Room deleted successfully!");
                }
                else
                {
                    return;
                }
            }

            LoadRooms();
        }

        private void btnManageSeats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Manage Seats feature is coming soon!");
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard = new AdminDashboardWindow();
            dashboard.Show();
            this.Close();
        }
    }
}