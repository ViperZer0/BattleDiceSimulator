// See https://aka.ms/new-console-template for more information

using BattleDice;
using BattleDice.Ability;

AbilityFactory abilityFactory = new AbilityFactory();
DiceFactory diceFactory = new DiceFactory(abilityFactory);
ResultManager resultManager = new ResultManager(diceFactory);
resultManager.MaxSamples = 4;
resultManager.CalculateAllResults();

