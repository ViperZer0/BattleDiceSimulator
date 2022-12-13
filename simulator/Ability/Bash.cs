namespace BattleDice.Ability
{
    class Bash: Ability
    {
        public Bash()
        {
            this.name = "Bash";
            this.priority = 7;
        }

        public override void Apply(Player caster)
        {
            if (caster.Target.IsAffected)
            {
                caster.Target.Damage(caster.CalcFocusMod(3));
            }
            else
            {
                caster.Target.Damage(caster.CalcFocusMod(1));
            }
        }
    }
}

