using System.CodeDom.Compiler;
using BattleDice.Ability;

namespace BattleDice;

public class ResultManager
{
    private List<Dice> dice;

    private AllPlayers players = new();

    private TotalResults allResults = new();

    private const string exportFile = "results.csv";

    private int numSamples = 0;

    public ResultManager(DiceFactory diceFactory)
    {
        using (StreamReader sr = new StreamReader("./res/dice.csv"))
        {
            dice = diceFactory.CreateAllDice(sr);
        }

        this.CreateAllPlayers();
    }

    public int MaxSamples
    {
        get;
        set;
    }
    
    public void CalculateAllResults()
    {
        int currentMatch = 0;
        // I'm sorry, this is gonna be really big.
        List <(Player, Player)> allMatches = this.GetAllMatches().ToList();
        int count = allMatches.Count();
        while (true)
        {
            var tasks = new Task[MaxSamples];
            for (int i = 0; i < MaxSamples; i++)
            {
                int pickMatch = RandomGenerator.Next(count);
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    this.CreateMatchup(allMatches[pickMatch].Item1, allMatches[pickMatch].Item2);
                });
                Console.WriteLine("Started sample {0}", i);
            }
            Task.WaitAll(tasks);
            Console.WriteLine("---------------------------------");
            PrintResults();
            using (var sw = new StreamWriter(exportFile))
            {
                allResults.ExportResults(sw);
            }

            if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        /*
    foreach (var pair in GetAllMatches())
    {
        this.CreateMatchup(pair.Item1,pair.Item2);
        currentMatch++;
        Console.WriteLine("{0}% done", (double)currentMatch/(double)end*(double)100);
        if (currentMatch % 100 == 0)
        {
            PrintResults();
        }
    }*/
    }

    public void PrintResults()
    {
        allResults.PrintResults();
        /*
        foreach (Results result in matchResults)
        {
            Console.WriteLine($"{result.Player.Name},{result.Against.Name}: {result.Wins},{result.Losses},{result.Ties}");
        }*/
    }

    private IEnumerable<(Player, Player)> GetAllMatches()
    {
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {
                if (!players.Players[i].SharesDiceWith(players.Players[j]))
                {
                    yield return (players.Players[i], players.Players[j]);
                }
            }
        }
    }

    private void CreateMatchup(Player player1, Player player2)
    {
        Mutex lock1 = players.Lock(player1);
        Mutex lock2 = players.Lock(player2);
        Matchup matchup = new Matchup(player1, player2)
        {
            MaxMatchTime = 1000,
            NumMatches = 1000,
        };
        
        matchup.Run();
        // This discard the "against" person. We only care about overall win rate.
        allResults.Add(matchup.Results);
        lock1.ReleaseMutex();
        lock2.ReleaseMutex();
    }

   private void CreateAllPlayers()
    {
        foreach ((Dice a, Dice b, Dice c) in GetAllDiceCombinations())
        {
            players.Add(new Player()
                {
                    Dice = new List<Dice>() { a, b, c }
                }
            );
        }
    }

    private IEnumerable<(Dice, Dice, Dice)> GetAllDiceCombinations()
    {
        int maxIndex = dice.Count;
        for (int i = 0; i < maxIndex-2; i++)
        {
            for (int j = i+1; j < maxIndex-1; j++)
            {
                for (int k = j + 1; k < maxIndex; k++)
                {
                    yield return new(dice[i], dice[j], dice[k]);
                }
            }
        }
    }
}