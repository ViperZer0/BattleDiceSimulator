namespace BattleDice.Ability
{
    class Curse: Ability
    {
        public Curse()
        {
            this.name = "Curse";
            this.priority = 7;
        }

        public override void Apply(Player caster)
        {
            caster.Target.Damage(caster.CalcFocusMod(3));
            caster.Cursed = true;
        }
    }
}
