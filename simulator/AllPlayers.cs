namespace BattleDice;

public class AllPlayers
{
    private List<Mutex> locks = new();

    private List<Player> players = new();

    public int Count
    {
        get => players.Count;
    }

    public List<Player> Players
    {
        get => this.players;
    }

    public void Add(Player player)
    {
        players.Add(player);
        locks.Add(new Mutex());
    }
    public Mutex Lock(Player player)
    {
        int index = players.FindIndex(i => i == player);
        locks[index].WaitOne();
        return locks[index];
    }
}