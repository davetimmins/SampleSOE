using System.Text;

namespace DaveTimmins.SampleExtension.Client
{
    internal static class Encoders
    {
        public static string UrlEncode(string str)
        {
            return str == null ? null : Encoding.ASCII.GetString(UrlEncodeBytes(Encoding.UTF8.GetBytes(str)));
        }

        private static byte[] UrlEncodeBytes(byte[] bytes)
        {
            int length = bytes.Length;
            int num1 = 0;
            int num2 = 0;
            for (int index = 0; index < length; ++index)
            {
                var ch = (char)bytes[index];
                if ((int)ch == 32)
                    ++num1;
                else if (!IsSafe(ch))
                    ++num2;
            }
            if (num1 == 0 && num2 == 0)
                return bytes;
            var numArray1 = new byte[length + num2 * 2];
            int num3 = 0;
            for (int index1 = 0; index1 < length; ++index1)
            {
                byte num4 = bytes[index1];
                var ch = (char)num4;
                if (IsSafe(ch))
                    numArray1[num3++] = num4;
                else if ((int)ch == 32)
                {
                    numArray1[num3++] = (byte)43;
                }
                else
                {
                    byte[] numArray2 = numArray1;
                    int index2 = num3;
                    int num5 = 1;
                    int num6 = index2 + num5;
                    int num7 = 37;
                    numArray2[index2] = (byte)num7;
                    byte[] numArray3 = numArray1;
                    int index3 = num6;
                    int num8 = 1;
                    int num9 = index3 + num8;
                    int num10 = (int)(byte)Encoders.IntToHex((int)num4 >> 4 & 15);
                    numArray3[index3] = (byte)num10;
                    byte[] numArray4 = numArray1;
                    int index4 = num9;
                    int num11 = 1;
                    num3 = index4 + num11;
                    int num12 = (int)(byte)Encoders.IntToHex((int)num4 & 15);
                    numArray4[index4] = (byte)num12;
                }
            }
            return numArray1;
        }

        private static char IntToHex(int n)
        {
            return n <= 9 ? (char)(n + 48) : (char)(n - 10 + 97);
        }

        private static bool IsSafe(char ch)
        {
            if ((int)ch >= 97 && (int)ch <= 122 || (int)ch >= 65 && (int)ch <= 90 || (int)ch >= 48 && (int)ch <= 57)
                return true;
            switch (ch)
            {
                case '!':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;
                default:
                    return false;
            }
        }
    }
}
