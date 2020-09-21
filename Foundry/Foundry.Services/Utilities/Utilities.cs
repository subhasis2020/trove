using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class Utilities
    {
        public static string GenerateNumToken(int numDigits)
        {
            Random generator = new Random();
            return generator.Next(0, 9999).ToString("D" + numDigits);
        }
    }
}
