namespace Common.Utils
{
    using System;

    public class ToolsMgr
    {
        public static int CalcByteSign(byte data)
        {
            if (data >= 0x80)
            {
                return (-128 + (data - 0x80));
            }
            return data;
        }

        public static string GetUUID() => 
            Guid.NewGuid().ToString();
    }
}
