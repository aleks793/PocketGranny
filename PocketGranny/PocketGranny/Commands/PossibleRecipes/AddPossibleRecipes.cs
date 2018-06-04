using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.PossibleRecipes
{
    public class AddPossibleRecipes : ICommand
    {
        private Application _app;

        private List<Recipe> _recipes;

        private ListCategoriesRecipes _listCategoriesRecipes;

        public AddPossibleRecipes(Application app, List<Recipe> recipes, ListCategoriesRecipes listCategoriesRecipes)
        {
            _app = app;
            _recipes = recipes;
            _listCategoriesRecipes = listCategoriesRecipes;
        }

        public string Name => "add";

        public string Help => "Добавить элемент в список желаемых рецептов";

        public string Description => "Параметры: индекс";

        public string[] Synonyms => new[] { "ADD" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                Console.WriteLine("Количество параметров должно быть равно одному");
                return;
            }

            if (!int.TryParse(parameters[0], out int index) && index >= _recipes.Count && index < 0)
            {
                Console.WriteLine($"Формат идентификатора { parameters[0] } не верен");
            }

            _listCategoriesRecipes.Add(_recipes[index]);
        }
    }
}