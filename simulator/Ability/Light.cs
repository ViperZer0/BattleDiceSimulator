namespace BattleDice.Ability
{
    class Light: Ability
    {
        public Light()
        {
            this.name = "Light";
            this.priority = 15;
        }

        public override void Apply(Player caster)
        {
            caster.AddLightRoll();
        }
    }
}
