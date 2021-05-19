using System;
using System.Linq;

namespace xycat.common
{
    public class Cipher
    {
        public static string Rot13(string value)
        {
            var array = value
                .ToCharArray()
                .Select(c => c switch
               {
                   >= 'a' and <= 'z' when c  > 'm' => Convert.ToChar(c - 13),
                   >= 'a' and <= 'z' when c <= 'm' => Convert.ToChar(c + 13),

                   >= 'A' and <= 'Z' when c  > 'M' => Convert.ToChar(c - 13),
                   >= 'A' and <= 'Z' when c <= 'M' => Convert.ToChar(c + 13),

                   _ => Convert.ToChar(c)
               })
               .ToArray();

            return new string(array);
        }
    }
}
