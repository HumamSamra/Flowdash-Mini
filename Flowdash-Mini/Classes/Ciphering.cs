using System.Security.Cryptography;
using System.Text;

namespace Flowdash_Mini.Classes
{
    public class Ciphering
    {
        static readonly string Key = "FlowDASEncKey2025";

        public static string Encrypt(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            byte[] bKey = Encoding.UTF8.GetBytes(Key);

            int CurrentKeyIndex = -1;

            List<byte> lstBytes = new();

            for (int iCounter = 0; iCounter < b.Length; iCounter++)
            {
                CurrentKeyIndex += 1;
                if (CurrentKeyIndex > bKey.Length - 1) CurrentKeyIndex = 0;
                int MyByte = b[iCounter] + bKey[CurrentKeyIndex];
                if (MyByte > 255) MyByte -= 255;
                lstBytes.Add((byte)MyByte);
            }

            return Convert.ToBase64String(lstBytes.ToArray());
        }

        public static string Decrypt(string str)
        {
            byte[] b = Convert.FromBase64String(str);
            byte[] bKey = Encoding.UTF8.GetBytes(Key);

            int CurrentKeyIndex = -1;

            List<byte> lstBytes = new();

            for (int iCounter = 0; iCounter < b.Length; iCounter++)
            {
                CurrentKeyIndex += 1;
                if (CurrentKeyIndex > bKey.Length - 1) CurrentKeyIndex = 0;
                int MyByte = b[iCounter] - bKey[CurrentKeyIndex];
                if (MyByte < 0) MyByte += 255;
                lstBytes.Add((byte)MyByte);
            }
            return Encoding.UTF8.GetString(lstBytes.ToArray());
        }

        public static string MD5Hash(string value)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(value);
            byte[] hashed = MD5.HashData(textBytes);
            return BitConverter.ToString(hashed).Replace("-", "").ToLowerInvariant();
        }

        public static string SHA256Hash(string value)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }

            return sb.ToString();
        }

    }
}
