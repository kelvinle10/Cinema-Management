using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CinemaManagementSystem.Service
{
    public class OTPService
    {
        public static string GenerateOTP()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000)
                .ToString();
        }
    }
}
