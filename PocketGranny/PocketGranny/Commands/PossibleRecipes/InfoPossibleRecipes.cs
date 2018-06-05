using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.PossibleRecipes
{
    public class InfoPossibleRecipes : ICommand
    {
        List<Recipe> _recipes;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public InfoPossibleRecipes(List<Recipe> recipes, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _recipes = recipes;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "info";

        public string Help => "Показать по индексу информацию по рецепту";

        public string Description => "Параметры: индекс";

        public string[] Synonyms => new[] { "INFO" };

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

            PrintRecipe(_recipes[index]);
        }

        private void PrintRecipe(Recipe recipe)
        {
            Console.WriteLine($"Recipe: { recipe.ToString() }");
            Console.WriteLine(Line);
            Console.WriteLine("Ingredients:");
            List<Commodity> products = recipe.GetProducts();

            foreach (var i in products)
            {
                Console.WriteLine(i.ToString());
            }

            Console.WriteLine(Line);
            Console.WriteLine("Info:");
            Console.WriteLine(recipe.Info);

            products = _availabilityProducts.GetMissingProducts(products);

            if (products.Count == 0)
            {
                Console.WriteLine("Продуктов достаточно");
                return;
            }

            Console.WriteLine("Не хватает этих продуктов:");

            foreach (var i in products)
            {
                Console.WriteLine(i.ToString());
            }

            Console.WriteLine("Добавить эти продукты в список необходимых продуктов?");
            var cmd = Console.ReadLine();

            if (cmd == "Y" || cmd == "y")
            {
                foreach (var i in products)
                {
                    _necessaryProducts.Add(i);
                }
            }
        }

        private const string Line = "================================================";
    }
}