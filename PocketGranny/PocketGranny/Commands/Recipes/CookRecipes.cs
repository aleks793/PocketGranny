using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.Recipes
{
    public class CookRecipes : ICommand
    {
        private ListCategoriesRecipes _listCategoriesRecipes;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        private ListConsumptionProducts _consumptionProducts;

        public CookRecipes(ListCategoriesRecipes listCategoriesRecipes, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts, ListConsumptionProducts consumptionProducts)
        {
            _listCategoriesRecipes = listCategoriesRecipes;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
            _consumptionProducts = consumptionProducts;
        }

        public string Name => "cook";

        public string Help => "Приготовить рецепт по индексу";

        public string Description => "Параметры: индекс";

        public string[] Synonyms => new[] { "COOK" };

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

                List<Commodity> products = recipe.GetProducts();
                List<Commodity> missingProducts = _availabilityProducts.GetMissingProducts(products);

                if (missingProducts.Count != 0)
                {
                    Console.WriteLine("Не хватает этих продуктов:");

                    foreach (var i in missingProducts)
                    {
                        Console.WriteLine(i.ToString());
                    }

                    Console.WriteLine("У вас есть эти продукты?(Y/)");
                    var cmd = Console.ReadLine();

                    if (cmd != "Y" && cmd != "y")
                    {
                        Console.WriteLine("Невозможно приготовить без продуктов");
                        Console.WriteLine("Добавить эти продукты в список необходимых продуктов?(Y/)");
                        var command = Console.ReadLine();

                        if (command == "Y" || command == "y")
                        {
                            foreach (var i in products)
                            {
                                _necessaryProducts.Add(i);
                            }
                        }

                        return;
                    }
                }

                _availabilityProducts.Add(new Commodity(new Product(recipe.CategoryName, recipe.Name, recipe.ExpiryIn, recipe.WeightUnit), recipe.Weight, DateTime.Today));
                
                foreach (var i in products)
                {
                    _consumptionProducts.Add(i.Product, DateTime.Today);
                    _consumptionProducts.ChangeElement(i.Product, i.Weight);
                }

                _listCategoriesRecipes.RemoveAt(levels);

                Console.WriteLine("Продукты необходимые для приготовления вычлись из доступных продуктов");
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
    }
}