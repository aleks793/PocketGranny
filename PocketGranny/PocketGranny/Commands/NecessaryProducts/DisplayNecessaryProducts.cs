using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class DisplayNecessaryProducts : ICommand
    {
        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        private ListConsumptionProducts _consumptionProducts;

        public DisplayNecessaryProducts(ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts, ListConsumptionProducts consumptionProducts)
        {
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
            _consumptionProducts = consumptionProducts;
        }

        public string Name => "display";

        public string Help => "Отображает список необходимых продуктов";

        public string Description => "";

        public string[] Synonyms => new[] { "DISPLAY" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            if (_necessaryProducts.Categories.Count == 0)
            {
                Console.WriteLine("Список необходимых продуктов пуст");
            }
            else
            {
                Console.WriteLine("Список для покупки:");

                _necessaryProducts.Print(false);
            }

            if (_consumptionProducts.Products.Count == 0)
            {
                return;
            }

            List<Commodity> products = _availabilityProducts.GetRecommendations(_consumptionProducts.RecommendedProducts());

            if (products.Count == 0)
            {
                return;
            }

            Console.WriteLine("Рекомендовано к покупке:");

            var recommendations = new ListCategoriesCommodity(DateTime.Today);

            recommendations.AddRange(products);
            recommendations.Print(false);
        }
    }
}