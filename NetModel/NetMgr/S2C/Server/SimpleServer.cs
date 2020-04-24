using System.Collections.Generic;

public class SimpleServer
{
    public int Id
    {
        get;
        set;
    }
    public string ServerIpAdd
    {
        get;
        set;
    }
    public int Port
    {
        get;
        set;
    }

    public virtual void StartServer()
    {

    }

    public virtual void StopServer()
    {

    }

}
