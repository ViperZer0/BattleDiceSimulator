namespace BattleDice.Ability
{
    class Poison: Ability
    {
        public Poison()
        {
            this.name = "Poison";
            this.priority = 2;
        }

        public override void Apply(Player caster)
        {
            caster.Target.AddEffect(Effect.Poison);
        }
    }
}

