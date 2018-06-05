using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.PossibleRecipes
{
    public class DisplayPossibleRecipes : ICommand
    {
        private List<Recipe> _recipes;

        public DisplayPossibleRecipes(List<Recipe> recipes)
        {
            _recipes = recipes;
        }

        public string Name => "display";

        public string Help => "Отображает список рецептов";

        public string Description => "";

        public string[] Synonyms => new[] { "DISPLAY" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            Console.WriteLine("Рецепты:");

            for (int i = 0; i < _recipes.Count; i++)
            {
                Console.WriteLine($"[{ i }] { _recipes[i].ToString() }");
            }
        }
    }
}