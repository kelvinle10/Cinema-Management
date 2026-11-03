
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class EditSeatWindow : Window
    {
        private int seatId;

        public EditSeatWindow(int id)
        {
            InitializeComponent();

            seatId = id;

            using (DBContext db = new DBContext())
            {
                Seat? seat = db.Seats.FirstOrDefault(s => s.SeatId == seatId);

                if (seat == null)
                {
                    MessageBox.Show("Seat not found!");
                    this.Close();
                    return;
                }

                Room? room = db.Rooms.FirstOrDefault(r => r.RoomId == seat.RoomId);

                txtRoomName.Text = "Room: " + room?.RoomName;
                txtSeatRow.Text = seat.SeatRow;
                txtSeatNumber.Text = seat.SeatNumber.ToString();
                txtSeatType.Text = seat.SeatType;
                chkIsActive.IsChecked = seat.IsActive;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
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
                Seat? seat = db.Seats.FirstOrDefault(s => s.SeatId == seatId);

                if (seat == null)
                {
                    MessageBox.Show("Seat not found!");
                    return;
                }

                bool isDuplicate = db.Seats.Any(s =>
                    s.RoomId == seat.RoomId &&
                    s.SeatRow == seatRow &&
                    s.SeatNumber == seatNumber &&
                    s.SeatId != seatId);

                if (isDuplicate)
                {
                    MessageBox.Show("This seat already exists in this room!");
                    return;
                }

                seat.SeatRow = seatRow;
                seat.SeatNumber = seatNumber;
                seat.SeatType = seatType;
                seat.IsActive = chkIsActive.IsChecked == true;

                db.SaveChanges();
            }

            MessageBox.Show("Seat updated successfully!");
            this.DialogResult = true;
            this.Close();
        }
    }
}
