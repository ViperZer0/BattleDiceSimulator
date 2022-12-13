namespace BattleDice.Ability
{
    class Dark: Ability
    {
        public Dark()
        {
            this.name = "Dark";
            this.priority = 5;
        }

        public override void Apply(Player caster)
        {
            caster.AddDarkRoll();
        }
    }
}
