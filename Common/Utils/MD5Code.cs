namespace Common.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class MD5Code
    {
        public static string GetMD5HashFromByte(byte[] data)
        {
            string str;
            try
            {
                byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(data);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(buffer[i].ToString("x2"));
                }
                str = builder.ToString();
            }
            catch (Exception exception)
            {
                throw new Exception("MD5校验失败" + exception.Message);
            }
            return str;
        }
    }
}
