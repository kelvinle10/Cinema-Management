using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class EditRoomWindow : Window
    {
        private int roomId;

        public EditRoomWindow(int id)
        {
            InitializeComponent();

            roomId = id;

            using (DBContext db = new DBContext())
            {
                Room? room = db.Rooms.FirstOrDefault(
                    r => r.RoomId == roomId);

                if (room == null)
                {
                    MessageBox.Show("Room not found!");
                    this.Close();
                    return;
                }

                txtRoomName.Text = room.RoomName;
                txtCapacity.Text = room.Capacity.ToString();
                chkIsActive.IsChecked = room.IsActive;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomName.Text) ||
                string.IsNullOrWhiteSpace(txtCapacity.Text))
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) ||
                capacity <= 0)
            {
                MessageBox.Show("Capacity must be a positive number!");
                return;
            }

            using (DBContext db = new DBContext())
            {
                Room? room = db.Rooms.FirstOrDefault(
                    r => r.RoomId == roomId);

                if (room == null)
                {
                    MessageBox.Show("Room not found!");
                    return;
                }

                room.RoomName = txtRoomName.Text.Trim();
                room.Capacity = capacity;
                room.IsActive = chkIsActive.IsChecked == true;

                db.SaveChanges();
            }

            MessageBox.Show("Room updated successfully!");

            this.DialogResult = true;
            this.Close();
        }
    }
}