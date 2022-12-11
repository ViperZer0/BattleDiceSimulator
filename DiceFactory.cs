using BattleDice.Ability;

namespace BattleDice;

public class DiceFactory
{
    private AbilityFactory factory;

    public DiceFactory(AbilityFactory factory)
    {
        this.factory = factory;
    }

    public Dice CreateDice(string line)
    {
        var tokens = line.Split(',');
        var name = tokens[0];
        List<Ability.Ability> abilities = new();
        Dice dice = new Dice(name, abilities);
        foreach (var token in tokens.Skip(1))
        {
            Ability.Ability ability = factory.CreateAbility(token);
            ability.Dice = dice;
            abilities.Add(ability);
        }

        return dice;
    }

    public List<Dice> CreateAllDice(TextReader reader)
    {
        List<Dice> allDice = new();
        string? line;
        do
        {
            line = reader.ReadLine();
            if (line != null)
            {
                allDice.Add(CreateDice(line));
            }
        } while (line != null);
        return allDice;
    }
}