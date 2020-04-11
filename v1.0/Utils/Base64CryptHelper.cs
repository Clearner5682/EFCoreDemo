using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class Base64CryptHelper
    {
        public static string Encrypt(string code,Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(code));
        }

        public static string Decrypt(string code,Encoding encoding)
        {
            int toFill = code.Length % 4;
            if (toFill > 0)
            {
                for(int i = toFill+1; i <= 4; i++)
                {
                    code += "=";
                }
            }
            byte[] bytes = Convert.FromBase64String(code);

            return encoding.GetString(bytes);
        }
    }
}
