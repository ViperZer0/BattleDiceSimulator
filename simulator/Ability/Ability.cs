using System;

namespace BattleDice.Ability {

abstract public class Ability: IComparable
{
    protected string name;

    protected int priority;

    public Dice Dice
    {
        get;
        set;
    }

    public int Priority
    {
        get => this.priority;
    }

    public string Name
    {
        get => this.name;
    }

    public virtual void Apply(Player caster){}
    
    public int CompareTo(object other)
    {
        if (other == null)
        {
            return 1;
        }

        Ability ability = other as Ability;
        if(ability != null)
        {
            return this.Priority.CompareTo(ability.Priority);
        }
        else
        {
            throw new ArgumentException("Object is not an ability");
        }
    }
}

}
