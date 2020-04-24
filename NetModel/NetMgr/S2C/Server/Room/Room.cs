using System.Collections;

public class Room
{
    public Room(int playersCount)
    {
        if (playersCount <= 0)
            throw new System.IndexOutOfRangeException("房间人数必须大于0");
        players = new RoomPlayer[playersCount];
        status = RoomStatus.WaitPlayer;
    }

    public int Id
    {
        get;
        set;
    }

    private RoomPlayer[] players;

    public RoomStatus status
    {
        private set;
        get;
    }

    public bool AddRoom(RoomPlayer player)
    {
        
    }

    public void QuitRoom(RoomPlayer player)
    {

    }

    
}

public enum RoomStatus
{
    None = 0,
    WaitPlayer = 1,
    Play = 2,
    Destory = 3
}
