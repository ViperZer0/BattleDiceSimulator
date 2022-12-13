namespace BattleDice.Ability
{
    class Meditate: Ability
    {
        public Meditate()
        {
             this.name = "Meditate";
             this.priority = 14;
        }

        public override void Apply(Player caster)
        {
            if (caster.Health < 6 &&
                    caster.GetCountOfEffect(Effect.Meditate) == 0)
            {
                caster.Heal(caster.CalcFocusMod(4));
                caster.AddSavedEffect(Effect.Meditate);
                this.Dice.Frozen = true;
            }
        }
    }
}


