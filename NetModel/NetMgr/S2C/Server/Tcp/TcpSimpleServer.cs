using System.Collections;

public class TcpSimpleServer : SimpleServer
{
    #region 处理客户端的连接请求
    private int curClientCount = 0;
    public int MaxOnLineCount
    {
        get;
        set;
    } = 50;

    public void AcceptConnect()
    {

    }

    private TcpSimpleClient CreateClient()
    {
        return null;
    }

    #endregion
}
