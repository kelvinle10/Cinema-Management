using CinemaManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CinemaManagementSystem.Views.Auth
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string phone = txtPhone.Text.Trim();
            DateOnly? dateOfBirth = dpDateOfBirth.SelectedDate.HasValue
                ? DateOnly.FromDateTime(dpDateOfBirth.SelectedDate.Value)
                : null;
            // Check empty fields
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            // Check email format
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Invalid email address!");
                return;
            }

            // Check password confirmation
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            using (DBContext db = new DBContext())
            {
                // Check duplicate username or email
                bool isDuplicate = db.Accounts.Any(a =>
                    a.Username == username ||
                    (a.Customer != null && a.Customer.Email == email));

                if (isDuplicate)
                {
                    MessageBox.Show("Username or email already exists!");
                    return;
                }

                // Hash password
                byte[] passwordHash = SHA256.HashData(
                    Encoding.UTF8.GetBytes(password));

                // Create Account
                Account account = new Account
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    FullName = fullName,
                    Role = "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                db.Accounts.Add(account);
                db.SaveChanges();

                // Create Customer
                CinemaManagementSystem.Models.Customer customer =
                    new CinemaManagementSystem.Models.Customer
                    {
                        AccountId = account.AccountId,
                        Email = email,
                        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                        DateOfBirth = dateOfBirth
                    };

                db.Customers.Add(customer);
                db.SaveChanges();
            }

            MessageBox.Show("Registration successful! Please login.");

            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
