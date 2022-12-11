using System.Globalization;
using System.Net.Sockets;

namespace BattleDice;

public class TotalResults
{
    // To make tuning the perceptron easier, we just want to record a player's overall win rate.
    private Dictionary<Player, (long wins, long losses)> results = new();

    private object resultLock = new();

    public void Add(Results result)
    {
        this.AddPlayerResults(result);
        this.AddEnemyResults(result);
    }

    private void AddPlayerResults(Results result)
    {
        if (results.ContainsKey(result.Player))
        {
            // We count ties as wins.
            long wins = results[result.Player].wins + result.Wins + result.Ties;
            long losses = results[result.Player].losses + result.Losses;
            lock (resultLock)
            {
                results[result.Player] = (wins, losses);
            }
        }
        else
        {
            lock (resultLock)
            {
                results[result.Player] = (result.Wins + result.Ties, result.Losses);
            }
        }       
    }
    
    private void AddEnemyResults(Results result)
    {
        if (results.ContainsKey(result.Against))
        {
            // We count ties as wins. "Against" wins = "Player" losses.
            long wins = results[result.Against].wins + result.Losses + result.Ties;
            long losses = results[result.Against].losses + result.Wins;
            lock (resultLock)
            {
                results[result.Against] = (wins, losses);
            }
        }
        else
        {
            lock (resultLock)
            {
                results[result.Against] = (result.Losses + result.Ties, result.Wins);
            }
        }       
    }
    
    public void PrintResults()
    {
        foreach (var result in results)
        {
            Console.WriteLine(result.Key.Name + ": " + result.Value.wins.ToString() + "," +
                              result.Value.losses.ToString());
        }
    }

    public void ExportResults(TextWriter tw)
    {
        lock (resultLock)
        {
            foreach (var result in results)
            {
                tw.WriteLine(result.Key.Name + "," +
                             ((double)result.Value.wins / (double)(result.Value.losses + result.Value.wins)).ToString(
                                 CultureInfo.InvariantCulture));
            }
        }
    }

}