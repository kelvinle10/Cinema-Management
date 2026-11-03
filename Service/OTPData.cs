using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaManagementSystem.Service
{
    public class OTPData
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
