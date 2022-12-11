namespace BattleDice.Ability
{
    class Growth: Ability
    {
        public Growth()
        {
            this.name = "Growth";
            this.priority = 13;
        }

        public override void Apply(Player caster)
        {
            caster.AddSavedEffect(Effect.Growth);
            caster.Heal(caster.CalcFocusMod(1));
        }
    }
}
