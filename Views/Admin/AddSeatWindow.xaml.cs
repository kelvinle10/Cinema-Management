
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class AddSeatWindow : Window
    {
        private int roomId;

        public AddSeatWindow(int id)
        {
            InitializeComponent();

            roomId = id;

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddSeat_Click(object sender, RoutedEventArgs e)
        {
            string seatRow = txtSeatRow.Text.Trim().ToUpper();
            string seatType = txtSeatType.Text.Trim();

            if (string.IsNullOrWhiteSpace(seatRow) ||
                string.IsNullOrWhiteSpace(txtSeatNumber.Text) ||
                string.IsNullOrWhiteSpace(seatType))
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            if (!int.TryParse(txtSeatNumber.Text, out int seatNumber) ||
                seatNumber <= 0)
            {
                MessageBox.Show("Seat Number must be a positive number!");
                return;
            }

            using (DBContext db = new DBContext())
            {
                bool isDuplicate = db.Seats.Any(s =>
                    s.RoomId == roomId &&
                    s.SeatRow == seatRow &&
                    s.SeatNumber == seatNumber);

                if (isDuplicate)
                {
                    MessageBox.Show("This seat already exists in this room!");
                    return;
                }

                Seat seat = new Seat
                {
                    RoomId = roomId,
                    SeatRow = seatRow,
                    SeatNumber = seatNumber,
                    SeatType = seatType,
                    IsActive = chkIsActive.IsChecked == true
                };

                db.Seats.Add(seat);
                db.SaveChanges();
            }

            MessageBox.Show("Seat added successfully!");
            this.DialogResult = true;
            this.Close();
        }
    }
}
