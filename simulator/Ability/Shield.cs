namespace BattleDice.Ability
{
    class Shield: Ability
    {
        public Shield()
        {
            this.name = "Shield";
            this.priority = 4;
        }

        public override void Apply(Player caster)
        {
            caster.AddSavedEffect(Effect.Shield);
        }
    }
}

