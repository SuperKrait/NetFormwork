using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
public class PackageFactory
{
    public static readonly PackageFactory Instance = new PackageFactory();
    public long curPackageId = 0;

    public PackageRequest GetRequest(ServerProtocolBase protocol)
    {        
        long tmpId = Interlocked.Add(ref curPackageId, 1);
        PackageRequest request = new PackageRequest(tmpId, protocol.ProtocolId, protocol.ProtocolSize);
        request.PackageId = tmpId;
        return request;
    }
}
