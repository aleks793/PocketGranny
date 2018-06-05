using System;
using ConsoleUI;

namespace PocketGranny.Commands.Recipes
{
    public class ClearRecipes : ICommand
    {
        private ListCategoriesRecipes _listCategoriesRecipes;

        public ClearRecipes(ListCategoriesRecipes listCategoriesRecipes)
        {
            _listCategoriesRecipes = listCategoriesRecipes;
        }

        public string Name => "clear";

        public string Help => "Очистить список желаемых рецептов";

        public string Description => "";

        public string[] Synonyms => new[] { "CLAER" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            _listCategoriesRecipes.Clear();
        }
    }
}