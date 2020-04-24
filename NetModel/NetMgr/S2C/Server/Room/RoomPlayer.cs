using System.Collections.Generic;
using System;
using System.Net.Sockets;

public class RoomPlayer
{
    public RoomPlayer(PlayerAccont player)
    {
        this.player = player;
        status = PlayerStatus.OnLine;
    }

    public int id;

    public PlayerAccont player;

    public TcpClient client;

    private PlayerStatus status = PlayerStatus.OffLine;

    private List<KeyValuePair<int, byte[]>> sendMessageList = new List<KeyValuePair<int, byte[]>>();

    protected virtual void SendTcpMessage(byte[] data)
    {

    }

    protected virtual byte[] GetTcpMessage()
    {
        return null;
    }

    public virtual void Update()
    {
        if (status == PlayerStatus.OffLine)
            return;
        if (client != null && client.Available > 0)
        {

        }
    }

    public void DestoryPlayer()
    {

    }
}
public enum PlayerStatus
{
    OffLine = 0,
    OnLine = 1,
}