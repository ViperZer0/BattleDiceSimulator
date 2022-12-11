using System;
using System.Collections.Generic;
using System.Reflection;

namespace BattleDice.Ability
{
    public class AbilityFactory
    {
        private Dictionary<string, Type> abilities = new();

        public AbilityFactory()
        {
            this.TraverseAvailableAbilities((name, type) => this.abilities.Add(name, type));
        }

        // Call this method when an ability is found and added to the list of abilities.
        private delegate void OnAbility(string name, Type type);

        public Ability CreateAbility(string name)
        {
            if (this.abilities.ContainsKey(name))
            {
                object? abilityObject = Activator.CreateInstance(this.abilities[name]);
                if (abilityObject is Ability ability)
                {
                    return ability;
                }
            }

            throw new ArgumentException("Unrecognized ability name {0}", name);
        }

        private void TraverseAvailableAbilities(OnAbility onAbility)
        {
            Type abilityType = typeof(Ability);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> abilityTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(abilityType));
                foreach (var type in abilityTypes)
                {
                    PropertyInfo? nameField  = type.GetProperty("Name");
                    if (nameField != null)
                    {
                        object? value = 
                            nameField.GetValue(Activator.CreateInstance(type));
                        if (value is string nameValue)
                        {
                            onAbility(nameValue, type);
                        }
                    }
                }
            }
        }
    }
}


