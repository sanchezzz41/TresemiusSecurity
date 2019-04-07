using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cipher
{
    public static class TresCipher
    {
        private static readonly string Alf = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        public static string Encrypt(string text, string key, int length = 10)
        {
            var key64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            var text64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

            var size = text64.Length / length < 1 ? 2 : text64.Length / length + 1;
            var resultTable = (key64 + Alf).Distinct().GetString().UpCountString(size);

            var result = "";
            for (int i = 0; i < text64.Length; i++)
            {
                var resultIndex = resultTable.IndexOf(text64[i]) + length + i;
                result += resultTable[resultIndex];
            }

            return result;
        }

        public static string Decrypt(string text, string key, int length = 10)
        {
            var key64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));

            var size = text.Length / length < 1 ? 2 : text.Length / length + 1;
            var resultTable = (key64 + Alf).Distinct().GetString().UpCountString(size);

            var result = "";
            for (int i = 0; i < text.Length; i++)
            {
                var resultIndex = resultTable.LastIndexOf(text[i]) - length - i;
                result += resultTable[resultIndex];
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(result));
        }
    }

    public static class Helper
    {
        public static string GetString(this IEnumerable<char> chars)
        {
            return String.Join("", chars);
        }

        public static string UpCountString(this string str, int count)
        {
            var result = "";
            for (int i = 0; i < count; i++)
            {
                result += str;
            }

            return result;
        }
    }
}
