using System;
using System.Collections.Generic;

public class PackageBase
{
    public PackageBase(long id, long size)
    {
        PackageId = id;
        PackageSize = size;
        PackageData = new byte[PackageSize];
        TimeTick = System.DateTime.Now.Ticks;
    }

    public long PackageId
    {
        get;
        set;
    }

    public long PackageSize
    {
        get;
        set;
    }

    public long TimeTick
    {
        get;
        set;
    }

    public byte[] PackageData
    {
        get;
        set;
    }

    public int ProtocolId
    {
        get;
        set;
    }

    /// <summary>
    /// 当前数据索引
    /// </summary>
    protected long curWriteIndex = 0;

    public int PackageHeaderLength
    {
        get;
        set;
    } = 76;


    public void SeekIndex(int i)
    {
        curWriteIndex = i;
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="data">要写入的字节数</param>
    /// <returns>成功写入了几个字节</returns>
    public void WriteData(byte[] data)
    {
        Array.Copy(data, 0, PackageData, curWriteIndex, data.Length);
        curWriteIndex += data.Length;
    }
}
