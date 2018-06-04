using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.Recipes
{
    public class InfoRecipes : ICommand
    {
        private Application _app;

        private ListCategoriesRecipes _listCategoriesRecipes;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public InfoRecipes(Application app, ListCategoriesRecipes listCategoriesRecipes, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _app = app;
            _listCategoriesRecipes = listCategoriesRecipes;
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

            var levelsString = parameters[0].Split(':');
            var levels = new int[levelsString.Length];

            try
            {
                for (var k = 0; k < levelsString.Length; k++)
                {
                    if (!int.TryParse(levelsString[k], out levels[k]))
                    {
                        throw new ArgumentException($"Формат идентификатора { parameters[0] } не верен");
                    }
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                var recipe = _listCategoriesRecipes.FindElement(levels);
                PrintRecipe(recipe);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Первое значение [{ levels[0] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
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