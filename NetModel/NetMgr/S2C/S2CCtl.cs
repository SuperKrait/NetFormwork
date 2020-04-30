using System.Collections.Generic;
using System;
using System.IO;
using System.Net;

public class S2CCtl
{
    #region 拆包模块

    private const long OnePackageMaxSize = 1024;
    private const string PackageKey = "Asd123";
    private readonly byte[] SpecialHeader = new byte[] {1, 2, 3, 4};

    private byte[][] GetPackageList(PackageRequest package)
    {
        long end = package.PackageSize % OnePackageMaxSize;
        long dataLength = OnePackageMaxSize - package.PackageHeaderLength;
        byte[][] list = new byte[end.Equals(0) ? 
            package.PackageSize / (dataLength) 
            : package.PackageSize / (dataLength) + 1][];

        long tmpIndex = 0;

        for (int i = 0; i < list.Length; i++)
        {
            using (MemoryStream steam = new MemoryStream(new byte[OnePackageMaxSize]))
            {
                using (BinaryWriter writer = new BinaryWriter(steam))
                {
                    writer.Write(SpecialHeader);
                    writer.Write(package.PackageSize);
                    writer.Write(package.PackageId);
                    writer.Write(i);
                    writer.Write(package.TimeTick);
                    writer.Write(new byte[32]);                    
                }
                list[i] = steam.GetBuffer();
            }
            Array.Copy(package.PackageData, tmpIndex, list[i], 0, dataLength);
            tmpIndex += package.PackageHeaderLength;
        }

        return list;
    }

    #endregion

    #region 发送模块

    public void AddSendQueue(PackageRequest package)
    {

    }

    #endregion

}
