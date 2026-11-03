
using CinemaManagementSystem.Models;
using CinemaManagementSystem.Service;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CinemaManagementSystem.Views.Auth
{
    public partial class ForgotPasswordWindow : Window
    {
        private OTPData otpData;
        private bool isOTPVerified = false;
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private async void btnSendOTP_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email!");
                return;
            }

            try
            {
                using (DBContext db = new DBContext())
                {
                    bool emailExists = db.Customers.Any(c => c.Email == email);

                    if (!emailExists)
                    {
                        MessageBox.Show("Email does not exist!");
                        return;
                    }
                }

                string otp = OTPService.GenerateOTP();

                otpData = new OTPData
                {
                    Email = email,
                    OTP = otp,
                    ExpiryTime = DateTime.Now.AddMinutes(5)
                };

                btnSendOTP.IsEnabled = false;

                await GmailService.SendEmailAsync(
                    email,
                    "Cinema Management System - Password Reset OTP",
                    $"Your OTP is: {otp}\n\n" +
                    "This code will expire in 5 minutes."
                );

                MessageBox.Show("OTP has been sent to your email!");

                EmailSection.Visibility = Visibility.Collapsed;
                OTPSection.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                otpData = null;
                MessageBox.Show("Failed to send OTP: " + ex.Message);
            }
            finally
            {
                btnSendOTP.IsEnabled = true;
            }
        }

        private void btnVerifyOTP_Click(object sender, RoutedEventArgs e)
        {
            string enteredOTP = txtOTP.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredOTP))
            {
                MessageBox.Show("Please enter the OTP!");
                return;
            }

            if (otpData == null || otpData.Email != txtEmail.Text.Trim())
            {
                MessageBox.Show("Please request a new OTP!");
                return;
            }

            if (DateTime.Now > otpData.ExpiryTime)
            {
                otpData = null;
                MessageBox.Show("OTP has expired! Please request a new one.");
                EmailSection.Visibility = Visibility.Visible;
                OTPSection.Visibility = Visibility.Collapsed;
                return;
            }

            if (enteredOTP != otpData.OTP)
            {
                MessageBox.Show("Incorrect OTP!");
                return;
            }

            isOTPVerified = true;

            OTPSection.Visibility = Visibility.Collapsed;
            ResetPasswordSection.Visibility = Visibility.Visible;

            MessageBox.Show("OTP verified successfully!");
        }

        private async void ResendOTP_Click(object sender, MouseButtonEventArgs e)
        {
            otpData = null;
            txtOTP.Clear();

            EmailSection.Visibility = Visibility.Visible;
            OTPSection.Visibility = Visibility.Collapsed;

            btnSendOTP_Click(sender, new RoutedEventArgs());
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmNewPassword.Password;

            if (!isOTPVerified || otpData == null)
            {
                MessageBox.Show("Please verify your OTP first!");
                return;
            }

            if (DateTime.Now > otpData.ExpiryTime)
            {
                MessageBox.Show("OTP has expired! Please request a new one.");
                otpData = null;
                isOTPVerified = false;

                ResetPasswordSection.Visibility = Visibility.Collapsed;
                EmailSection.Visibility = Visibility.Visible;

                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            byte[] passwordHash = SHA256.HashData(
                Encoding.UTF8.GetBytes(newPassword));

            using (DBContext db = new DBContext())
            {
                var customer = db.Customers
                    .FirstOrDefault(c => c.Email == otpData.Email);

                if (customer == null)
                {
                    MessageBox.Show("Account does not exist!");
                    return;
                }

                var account = db.Accounts
                    .FirstOrDefault(a => a.AccountId == customer.AccountId);

                if (account == null)
                {
                    MessageBox.Show("Account does not exist!");
                    return;
                }

                account.PasswordHash = passwordHash;
                db.SaveChanges();
            }

            otpData = null;
            isOTPVerified = false;

            MessageBox.Show("Password reset successfully! Please login.");

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

    }
}
