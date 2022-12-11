namespace BattleDice.Ability
{
    class Focus: Ability
    {
        private float saveChance = 0.5F;

        public Focus()
        {
            this.name = "Focus";
            this.priority = 1;
        }

        public override void Apply(Player caster)
        {
            if (RandomGenerator.NextDouble() < this.saveChance)
            {
                caster.AddSavedEffect(Effect.Focus);
            }
            else
            {
                caster.AddEffect(Effect.Focus);
            }
        }
    }
}



        
