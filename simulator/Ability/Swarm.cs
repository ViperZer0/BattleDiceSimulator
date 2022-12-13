namespace BattleDice.Ability
{
    class Swarm: Ability
    {
        public Swarm()
        {
            this.name = "Swarm";
            this.priority = 11;
        }

        public override void Apply(Player caster)
        {
            caster.AddSavedEffect(Effect.Swarm);
            caster.Target.Damage(caster.CalcFocusMod(1));
        }
    }
}
