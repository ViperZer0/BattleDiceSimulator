namespace BattleDice;

public class Dice
{
    private List<Ability.Ability> abilities;

    public Dice(string name, List<Ability.Ability> abilities)
    {
        this.Name = name;
        this.abilities = abilities;
    }

    public string Name
    {
        get;
        private set;
    }
    public bool Frozen
    {
        get;
        set;
    }

    /// <summary>
    /// Causes this dice to be frozen on its next turn.
    /// </summary>
    public bool Meditating
    {
        get;
        set;
    }

    public Ability.Ability? Roll()
    {
       if (!Frozen)
       {
           int result = RandomGenerator.Next(abilities.Count);
           return abilities[result];
       }
       else
       {
           return null;
       }
    }
}