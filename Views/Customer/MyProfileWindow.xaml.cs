using CinemaManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class MyProfileWindow : Window
    {
        private readonly int customerId;

        public MyProfileWindow(int customerId)
        {
            InitializeComponent();

            this.customerId = customerId;

            LoadProfile();
        }

        // Load customer information
        private void LoadProfile()
        {
            using (DBContext db = new DBContext())
            {
                var customer = db.Customers
                    .Include(c => c.Account)
                    .FirstOrDefault(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    MessageBox.Show("Customer not found!");
                    Close();
                    return;
                }

                txtFullName.Text = customer.Account.FullName;
                txtUsername.Text = customer.Account.Username;
                txtEmail.Text = customer.Email;
                txtPhone.Text = customer.Phone ?? "";

                if (customer.DateOfBirth.HasValue)
                {
                    dpDateOfBirth.SelectedDate =
                        customer.DateOfBirth.Value.ToDateTime(
                            TimeOnly.MinValue);
                }
                else
                {
                    dpDateOfBirth.SelectedDate = null;
                }
            }
        }

        // Save profile changes
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName)
                || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show(
                    "Full Name and Email are required!",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show(
                    "Please enter a valid email address!");
                return;
            }

            using (DBContext db = new DBContext())
            {
                var customer = db.Customers
                    .Include(c => c.Account)
                    .FirstOrDefault(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    MessageBox.Show("Customer not found!");
                    return;
                }

                bool emailExists = db.Customers.Any(c =>
                    c.CustomerId != customerId
                    && c.Email.ToLower() == email.ToLower());

                if (emailExists)
                {
                    MessageBox.Show("This email is already in use!");
                    return;
                }

                customer.Account.FullName = fullName;
                customer.Email = email;
                customer.Phone = string.IsNullOrWhiteSpace(phone)
                    ? null
                    : phone;

                customer.DateOfBirth = dpDateOfBirth.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(
                        dpDateOfBirth.SelectedDate.Value)
                    : null;

                db.SaveChanges();
            }

            MessageBox.Show(
                "Profile updated successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadProfile();
        }

        // Back
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}