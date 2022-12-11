using System.Transactions;

namespace BattleDice;

public static class RandomGenerator
{
    private static Object randLock = new();

    private static Random random = new Random();
    //private static Random random = new XORShiftRandom();

    public static double NextDouble()
    {
        lock (randLock)
        {
            return random.NextDouble();
        }
    }

    public static double Next()
    {
        lock (randLock)
        {
            return random.Next();
        }
    }

    public static int Next(int max)
    {
        lock (randLock)
        {
            return random.Next(max);
        }
    }

}