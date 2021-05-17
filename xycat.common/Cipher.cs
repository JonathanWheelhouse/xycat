using System;

namespace xycat.common
{
    public class Cipher
    {
        public static string Rot13(string value)
        {
            char[] array = value.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                var transposed = number switch
                {
                    _ when (number >= 'a' && number <= 'z') => number > 'm' ? number - 13 : number + 13,
                    _ when (number >= 'A' && number <= 'Z') => number > 'M' ? number - 13 : number + 13,
                    _ => number
                };

                array[i] = (char)transposed;
            }
            return new string(array);
        }
    }
}
