using System.Collections;

public class PlayerClient
{
    public ClientStatus Status
    {
        set;
        get;

    } = ClientStatus.OffLine;

    public virtual void Update()
    {

    }

    #region 连接模块

    private ServerInfo CurServer
    {
        get;
        set;
    }
    public ServerInfo LastServer
    {
        get;
        set;
    }


    public virtual void SearchServer()
    {
    }

    public virtual void ConnectServer(ServerInfo server)
    {

    }

    public virtual void EndServer()
    {
    }

    private void CheckConnect()
    {

    }

    public virtual void ReConnect()
    {

    }

    #endregion

    #region 发送指令

    protected void SendMessage2Server()
    {

    }

    #endregion

    #region 接受指令

    protected void GetMessageFromServer()
    {

    }

    #endregion
}


public enum ClientStatus
{
    OffLine = 0,
    OnLine = 1
}

