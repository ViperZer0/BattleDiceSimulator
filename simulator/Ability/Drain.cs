namespace BattleDice.Ability
{
    class Drain: Ability
    {
        private float addDamageChance = 0.5F;

        public Drain()
        {
            this.name = "Drain";
            this.priority = 10;
        }

        public override void Apply(Player caster)
        {
            caster.Drain(addDamageChance);
        }
    }
}



