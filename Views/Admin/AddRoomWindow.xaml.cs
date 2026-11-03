using CinemaManagementSystem.Models;
using System;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class AddRoomWindow : Window
    {
        public AddRoomWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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

            Room room = new Room
            {
                RoomName = txtRoomName.Text.Trim(),
                Capacity = capacity,
                IsActive = chkIsActive.IsChecked == true
            };

            using (DBContext db = new DBContext())
            {
                db.Rooms.Add(room);
                db.SaveChanges();
            }

            MessageBox.Show("Room added successfully!");

            this.DialogResult = true;
            this.Close();
        }
    }
}