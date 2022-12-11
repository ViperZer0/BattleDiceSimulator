namespace BattleDice;

public class Matchup
{
    private List<Player> players = new();

    private Results results;

    public Matchup(Player player1, Player player2)
    {
        this.results = new Results(player1, player2);
        this.players.Add(player1);
        this.players.Add(player2);
        // "Connect" the players so that each player targets the one after it.
        for (int i = 0; i < this.players.Count; i++)
        {
            this.players[i].Target = this.players[(i + 1) % this.players.Count];
        }
    }

    public int NumMatches
    {
        get;
        set;
    }

    public int MaxMatchTime
    {
        get;
        set;
    }

    public Results Results
    {
        get => this.results;
    }

    public void Run()
    {
        
        for (int i = 0; i < NumMatches; i++)
        {
            this.ExecuteSingleMatch();
        }
    }

    public void ExecuteSingleMatch()
    {
        foreach (Player player in players)
        {
            // Return each player to a clean state.
            player.Init();
        }

        int turnNumber = 0;
        int playerCount = 0;
        while (turnNumber < MaxMatchTime)
        {
            playerCount = 0;
            // Each player takes their turn.
            foreach (Player player in players.Where(player => !player.PermaDead)) 
            {
                player.TakeTurn();
                playerCount++;
            }

            // Filter out dead players.
            if (playerCount == 0)
            {
                this.results.Ties += 1;
                break;
            }

            // only works with 1v1 duels right now.
            if (playerCount == 1)
            {
                if (!players[0].PermaDead)
                {
                    this.results.Wins += 1;
                }
                else
                {
                    this.results.Losses += 1;
                }

                break;
            }

            turnNumber++;
        }

        // Running out of time counts as a tie.
        if (turnNumber >= MaxMatchTime)
        {
            this.results.Ties += 1;
        }
    }
}