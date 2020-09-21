using System;
using System.Security.Cryptography;
using System.Text;

namespace Foundry.Domain
{
    public class Cryptography
    {
               
        /// <summary>
        /// This method is used to convert the plain text to Encrypted/Un-Readable Text format.
        /// </summary>
        /// <param name="PlainText">Plain Text to Encrypt before transferring over the network.</param>
        /// <returns>Cipher Text</returns>
        public static string EncryptPlainToCipher(string stringvalue)
        {
            Byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(stringvalue);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        /// <summary>
        /// This method is used to convert the encrypted text to plain readable Text format.
        /// </summary>
        /// <param name="PlainText">Encrypt to Plain Text before transferring over the network.</param>
        /// <returns>Cipher Text</returns>
        public static string DecryptCipherToPlain(string hexvalue)
        {
            int CharsLength = hexvalue.Length;
            byte[] bytesarray = new byte[CharsLength / 2];
            for (int i = 0; i < CharsLength; i += 2)
            {
                bytesarray[i / 2] = Convert.ToByte(hexvalue.Substring(i, 2), 16);
            }
            return System.Text.Encoding.Unicode.GetString(bytesarray);
        }
        public static long GetNextInt64()
        {
            var bytes = new byte[sizeof(Int64)];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(bytes);

            long random = BitConverter.ToInt64(bytes, 0);

            //Remove any possible negative generator numbers and shorten the generated number to 12-digits
            string pos = random.ToString().Replace("-", "").Substring(0, 12);

            return Convert.ToInt64(pos);
        }
    }
}
