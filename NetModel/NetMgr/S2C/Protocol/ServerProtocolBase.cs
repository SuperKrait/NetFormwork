using System;
using System.Collections;

public class ServerProtocolBase
{
    public ServerProtocolBase(int id, int size)
    {
        ProtocolId = id;
        ProtocolSize = size;
    }

    public int ProtocolId
    {
        get;
        set;
    }

    public int ProtocolSize
    {
        get;
        set;
    }
}
