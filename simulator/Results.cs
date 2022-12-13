namespace BattleDice;

/// <summary>
/// This currently only works with two players (1v1) matchups.
/// </summary>
public class Results
{
    public Results(Player player, Player against)
    {
        this.Player = player;
        this.Against = against;
        this.Wins = 0;
        this.Losses = 0;
        this.Ties = 0;
    }

    public Player Player
    {
        get;
        set;
    }

    public Player Against
    {
        get;
        set;
    }
    public int Wins
    {
        get;
        set;
    }

    public int Losses
    {
        get;
        set;
    }

    public int Ties
    {
        get;
        set;
    }

    public int TotalMatches
    {
        get => this.Wins + this.Losses + this.Ties;
    }
    
    public string ToReadableString()
    {
        return Player.Name + " vs. " + Against.Name + "\tWins: " + Wins.ToString() + "\tLosses: " + Losses.ToString() +
               "\tTies: " + Ties.ToString() + "\n";
    }

    public string ToCSVString()
    {
        return Player.Name + "," + Against.Name + "," + Wins.ToString() + "," + Losses.ToString() +
               "," + Ties.ToString() + "\n";
    }

}