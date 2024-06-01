using System.Security.Cryptography;
using System.Text;

namespace FEKDMETAK2.Models
{
    public class HashPassword
    {
        public static string Hashpassword(string password)
        {
            string ps = string.Empty;
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            ps = builder.ToString();
            return ps;
        }
    }
}
