namespace BattleDice.Ability
{
    class Fire: Ability
    {
        public Fire()
        {
            this.name = "Fire";
            this.priority = 6;
        }

        public override void Apply(Player caster)
        {
            caster.Target.PureDamage(caster.CalcFocusMod(2));
        }
    }
}
