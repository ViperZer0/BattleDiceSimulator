namespace BattleDice.Ability
{
    class Lightning: Ability
    {
        public Lightning()
        {
            this.name = "Lightning";
            this.priority = 9;
        }

        public override void Apply(Player caster)
        {
            // Every die with lightning has a 1/3 chance of applying.
            caster.Target.Damage(caster.CalcFocusMod(1));
            while (RandomGenerator.Next(3) == 0)
            {
                caster.Target.Damage(caster.CalcFocusMod(1));
            }
        }
    }
}

