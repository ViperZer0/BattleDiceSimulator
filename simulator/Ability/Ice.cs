namespace BattleDice.Ability
{
    class Ice: Ability
    {
        private float applyToNextChance = 0.15F;

        private Random random = new(); 
        public Ice()
        {
            this.name = "Ice";
            this.priority = 3;
        }

        public override void Apply(Player caster)
        {
            caster.Target.Damage(caster.CalcFocusMod(1));
            if (random.NextDouble() < this.applyToNextChance)
            {
                caster.Target.AddEffect(Effect.Ice);
            }
            else
            {
                caster.Previous.AddEffect(Effect.Ice);
            }
        }
    }
}



