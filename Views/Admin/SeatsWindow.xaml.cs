
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class SeatsWindow : Window
    {
        private int roomId;

        public SeatsWindow(int id)
        {
            InitializeComponent();

            roomId = id;
            LoadRoomInfo();
            LoadSeats();
        }

        private void LoadRoomInfo()
        {
            using (DBContext db = new DBContext())
            {
                Room? room = db.Rooms.FirstOrDefault(r => r.RoomId == roomId);

                if (room == null)
                {
                    MessageBox.Show("Room not found!");
                    this.Close();
                    return;
                }

                txtRoomName.Text = "Room: " + room.RoomName;
            }
        }

        private void LoadSeats()
        {
            using (DBContext db = new DBContext())
            {
                dgSeats.ItemsSource = db.Seats
                    .Where(s => s.RoomId == roomId)
                    .OrderBy(s => s.SeatRow)
                    .ThenBy(s => s.SeatNumber)
                    .ToList();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            using (DBContext db = new DBContext())
            {
                dgSeats.ItemsSource = db.Seats
                    .Where(s => s.RoomId == roomId &&
                        (s.SeatRow.ToLower().Contains(keyword) ||
                         s.SeatNumber.ToString().Contains(keyword)))
                    .OrderBy(s => s.SeatRow)
                    .ThenBy(s => s.SeatNumber)
                    .ToList();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            RoomsWindow roomsWindow = new RoomsWindow();
            roomsWindow.Show();
            this.Close();
        }


        private void btnAddSeat_Click(object sender, RoutedEventArgs e)
        {
            AddSeatWindow addWindow = new AddSeatWindow(roomId);

            if (addWindow.ShowDialog() == true)
            {
                LoadSeats();
            }
        }



        private void btnEditSeat_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            Seat? selectedSeat = button?.DataContext as Seat;

            if (selectedSeat == null) return;

            EditSeatWindow editWindow = new EditSeatWindow(selectedSeat.SeatId);

            if (editWindow.ShowDialog() == true)
            {
                LoadSeats();
            }
        }



        private void btnDeleteSeat_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            Seat? selectedSeat = button?.DataContext as Seat;

            if (selectedSeat == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete seat {selectedSeat.SeatRow}{selectedSeat.SeatNumber}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            using (DBContext db = new DBContext())
            {
                Seat? seat = db.Seats.FirstOrDefault(s => s.SeatId == selectedSeat.SeatId);

                if (seat == null)
                {
                    MessageBox.Show("Seat not found!");
                    return;
                }

                bool hasTickets = db.Tickets.Any(t => t.SeatId == seat.SeatId);

                if (hasTickets)
                {
                    MessageBox.Show(
                        "Cannot delete this seat because it has related tickets!",
                        "Delete Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                db.Seats.Remove(seat);
                db.SaveChanges();
            }

            MessageBox.Show("Seat deleted successfully!");
            LoadSeats();
        }

    }
}
