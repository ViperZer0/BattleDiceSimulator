using System.Collections.Generic;
using System.Text;

namespace BattleDice
{
    public class Player
    {
        private List<Dice> dice = new();

        private int health = 10;

        public List<Dice> Dice
        {
            get => this.dice;
            set
            {
                this.dice = value;
            }
        }

        public int Health
        {
            get => this.health;
            set {
                if (value < 0)
                {
                    this.health = 0;
                }

                if (value > 10)
                {
                    this.health = 10;
                }
                else
                {
                    this.health = value;
                }
            }
        }

        private Player target;
        
        public Player Target
        {
            get => this.target;
            set
            {
                this.target = value;
                value.Previous = this;
            }
        }

        private Player prev;

        private Ability.Ability[] abilityResults = new Ability.Ability[3];
        
        public Player Previous
        {
            get => this.prev;
            set => this.prev = value;
        }

        private int deaths = 0;

        public int Deaths
        {
            get => this.deaths;
            set {
                if (value < 0)
                {
                    throw new ArgumentException("Deaths can't be set to a negative amount!");
                }
                this.deaths = value;
            }
        }

        private int shieldHealth =0;

        private int darkRolls = 0;

        private int lightRolls = 0;

        // If cursed, we take 1 self damage ONLY, no matter how many curses we roll.
        private bool cursed = false;

        public bool Cursed
        {
            get => this.cursed;
            set => this.cursed = value;
        }

        private bool dead = false;

        private bool permaDead = false;

        public bool PermaDead{
            get => this.permaDead;
        }
 
        private Dictionary<Effect, int> effects = new();
    
        public Dictionary<Effect, int> Effects
        {
            get => this.effects;
        }

        public bool IsAffected
        {
            get => this.effects.Count > 0;
        }

        private Dictionary<Effect, int> effectQueue = new();

        public string Name
        {
            get
            {
                return Dice.Select(i => i.Name).Aggregate((i, j) => i + "," + j);
            }
        }

        public string Rolls
        {
            get
            {
                return this.abilityResults.Select(i => i != null ? i.Name : string.Empty).Aggregate((i, j) => i + "," + j);
            }
        }

        public void Init()
        {
            this.health = 10;
            this.deaths = 0;
            this.shieldHealth = 0;
            this.darkRolls = 0;
            this.lightRolls = 0;
            this.cursed = false;
            this.permaDead = false;
            this.effects.Clear();
            this.effectQueue.Clear();
        }

        public void TakeTurn()
        {
            this.Setup();
            this.Roll();
            this.ApplyResults();
            this.Cleanup();
        }

        public void Setup()
        {
            // If the player is at 0 at the beginning of their turn, they are
            // temporarily deadified.
            if (this.health == 0)
            {
                this.deaths += 1;
                this.dead = true;
            }

            this.shieldHealth = 0;

            // Freeze some dice randomly.
            var toFreeze = dice.OrderBy(x => RandomGenerator.Next()).Take(GetCountOfEffect(Effect.Ice));
            foreach(var dice in toFreeze)
            {
                dice.Frozen = true;
            }
        }

        public void Roll()
        {
            int dicePoolCount = 0;
            // Count number of non frozen dice.
            foreach (Dice dice in this.dice)
            {
                if (!dice.Frozen)
                {
                    dicePoolCount++;
                }
            }

            int maxDice = 2;
            // If we have either growth or swarm applied we can have 3 dice.
            if (this.GetCountOfEffect(Effect.Growth) > 0 || this.GetCountOfEffect(Effect.Swarm) > 0)
            {
                maxDice = 3;
            }

            // Max dice can't exceed number of dice we own (that aren't frozen)
            if (dicePoolCount < maxDice)
            {
                maxDice = dicePoolCount;
            }

            if (dicePoolCount != maxDice)
            {
                // The actual dice we are rolling. Take a random set of those dice, limited to non-frozen dice.
                int index = 0;
                int diceIndex = 0;
                int selectNeeded = maxDice;
                int diceLeft = dicePoolCount;
                // This should never happen but maybe it'll happen idkk
                while (selectNeeded > 0 && diceIndex < dice.Count)
                {
                    // We have a (number needed)/(number left) probability of picking an element
                    // dice left decreases with each element we move over, chosen or not,
                    // and selectNeeded only decreases with each element selected.
                    bool pickThis = RandomGenerator.Next(diceLeft) < selectNeeded;
                    if (pickThis && !dice[diceIndex].Frozen)
                    {
                        abilityResults[index] = dice[diceIndex].Roll();
                        selectNeeded--;
                        index++;
                    }

                    diceIndex++;
                    diceLeft--;
                }
                
            }
            else
            {
                int index = 0;
                // We don't have to shuffle if we are using all the dice.
                foreach (Dice dice in dice)
                {
                    if (!dice.Frozen)
                    {
                        abilityResults[index] = dice.Roll();
                        index++;
                    }
                }
            }
        }

        public void ApplyResults()
        {
            Array.Sort(this.abilityResults);
            foreach(var result in abilityResults)
            {
                if (result != null)
                {
                    result.Apply(this);
                }
            }
        }

        public void Cleanup()
        {
            // Are we cursed?
            if (this.Cursed)
            {
                this.SelfDamage(1);
            }

            this.Cursed = false;

            int darkDamage;
            int lightDamage;

            switch (lightRolls)
            {
                case 0:
                    lightDamage = 0;
                    break;
                case 1:
                    lightDamage = 1;
                    break;
                case 2:
                    lightDamage = 4;
                    break;
                case 3:
                    lightDamage = 10;
                    break;
                default:
                    lightDamage = 10;
                    break;
            }

            switch (darkRolls)
            {
                case 0:
                    darkDamage = 0;
                    break;
                case 1:
                    darkDamage = 1;
                    break;
                case 2:
                    darkDamage = 5;
                    break;
                case 3:
                    darkDamage = 13;
                    break;
                default:
                    darkDamage = 13;
                    break;
            }   
             
            this.Target.Damage(this.CalcFocusMod(darkDamage));
            this.LightHeal(this.CalcFocusMod(lightDamage));

            // Clear currently applied effects, apply saved effects (shield and saved focus)
            this.effects.Clear();
            // Copy effect queue to effects.
            foreach (var pair in this.effectQueue)
            {
                this.effects[pair.Key] = pair.Value;
            }
            this.effectQueue.Clear();
            
            // Wipe abilityEffect
            for (int i = 0; i < 3; i++)
            {
                this.abilityResults[i] = null;
            }

            this.shieldHealth = 3*this.GetCountOfEffect(Effect.Shield);
            this.lightRolls = 0;
            this.darkRolls = 0;
            foreach (var dice in this.dice)
            {
                dice.Frozen = false;
                if (dice.Meditating)
                {
                    dice.Frozen = true;
                    dice.Meditating = false;
                }
            }
            
            if (this.dead && this.Health < this.Deaths)
            {
                this.permaDead = true;
            }
            else
            {
                this.dead = false;
            }
        }

        public void AddDarkRoll()
        {
            this.darkRolls += 1;
            if (this.darkRolls > 3)
            {
                this.darkRolls = 3;
            }
        }

        public void AddLightRoll()
        {
            this.lightRolls += 1;
            if (this.lightRolls > 3)
            {
                this.lightRolls = 3;
            }
        }

        public int CalcFocusMod(int b)
        {
            return b * (int)Math.Pow(2, this.GetCountOfEffect(Effect.Focus));
        }

        // Normal damage.
        public void Damage(int amount)
        {
            if (this.GetCountOfEffect(Effect.Shield) > 0)
            {
                this.PureDamage(this.SubtractShieldHealth(amount));
            }
            else
            {
                this.PureDamage(amount);
            }
        }
        
        // Don't calculate deaths here.
        // Bypass shield damage.
        public void PureDamage(int amount)
        {
            if (amount > 0){
                this.Health -= amount;
            }
        }

        // Only curse uses this.
        public void SelfDamage(int amount)
        {
            if (this.GetCountOfEffect(Effect.Poison) > 0)
            {
                this.PureHeal(amount);
            }
            else
            {
                this.PureDamage(amount);
            }
        }

        // Heal but take poison into account.
        public void Heal(int amount)
        {
            if (this.GetCountOfEffect(Effect.Poison) > 0)
            {
                this.PureDamage(amount);
            }
            else
            {
                this.PureHeal(amount);
            }
        }

        /// <summary>
        /// This method is used for light healing.
        /// It uses the overheal to damage the next opponent.
        /// </summary>
        /// <param name="amount">The amount to heal by.</param>
        public void LightHeal(int amount)
        {
            if (this.GetCountOfEffect(Effect.Poison) == 0)
            {
                int overheal = (this.health + amount - 10);
                if (overheal > 0)
                {
                    this.Target.Damage(overheal);
                }
            }
            this.Heal(amount);
        }

        // Heal but don't account for poison effect.
        public void PureHeal(int amount)
        {
            this.Health += amount;
        }

        /// <summary>
        /// Calculate drain damage.
        /// </summary>
        public void Drain(float addDamageChance)
        {
                        int healAmount = 1;
            int damageAmount = 1;
            for (int i = 0; i<this.GetCountOfEffect(Effect.Focus); i++)
            {
                healAmount *= 2;
                damageAmount *= 2;
                if (RandomGenerator.NextDouble() < addDamageChance)
                {
                    damageAmount += 1;
                }
                else
                {
                    healAmount += 1;
                }
            }

            this.Target.Damage(damageAmount);
            this.Heal(healAmount);
        }

        public bool SharesDiceWith(Player player2)
        {
            foreach (Dice dice in this.Dice)
            {
                if (player2.Dice.Contains(dice))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddEffect(Effect effect)
        {
            if (this.effects.ContainsKey(effect))
            {
                this.Effects[effect] += 1;
            }
            else
            {
                this.Effects[effect] = 1;
            }
        }

        public void AddSavedEffect(Effect effect)
        {
            if (this.effectQueue.ContainsKey(effect))
            {
                this.effectQueue[effect] += 1;
            }
            else
            {
                this.effectQueue[effect] = 1;
            }
        }

        public int GetCountOfEffect(Effect effect)
        {
            if (this.Effects.ContainsKey(effect))
            {
                return this.Effects[effect];
            }
            else
            {
                return 0;
            }
        }

        private int SubtractShieldHealth(int damage)
        {
            if (damage < this.shieldHealth)
            {
                this.shieldHealth -= damage;
                return 0;
            }
            else
            {
                damage = damage - this.shieldHealth;
                this.shieldHealth = 0;
                return damage;
            }
        }
    }
}
