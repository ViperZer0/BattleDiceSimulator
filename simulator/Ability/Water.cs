namespace BattleDice.Ability
{
    class Water: Ability
    {
        public Water()
        {
            this.name = "Water";
            this.priority = 12;
        }

        public override void Apply(Player caster)
        {
            if (caster.Health == 0)
            {
                caster.Heal(caster.CalcFocusMod(3));
            }
            else
            {
                caster.Heal(caster.CalcFocusMod(2));
            }
        }
    }
}
