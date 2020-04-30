using System;
using System.Collections;

public class ServerProtocolRequest : ServerProtocolBase
{
    public ServerProtocolRequest(int id, int size) : base(id, size)
    {
        
    }

    protected virtual PackageRequest WriteData()
    {
        PackageRequest request = PackageFactory.Instance.GetRequest(this);

        return request;
    }

    public virtual void Send()
    {
        PackageRequest request = WriteData();

    }
}
