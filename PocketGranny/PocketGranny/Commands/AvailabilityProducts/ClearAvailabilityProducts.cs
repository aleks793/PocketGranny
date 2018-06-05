using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.AvailabilityProducts
{
    public class ClearAvailabilityProducts : ICommand
    {
        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        private ListConsumptionProducts _consumptionProducts;

        private AvailableRecipes _availableRecipes;

        public ClearAvailabilityProducts(ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts, ListConsumptionProducts consumptionProducts, AvailableRecipes availableRecipes)
        {
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
            _consumptionProducts = consumptionProducts;
            _availableRecipes = availableRecipes;

        }

        public string Name => "clear";

        public string Help => "Очистить список доступных продуктов";

        public string Description => "";

        public string[] Synonyms => new[] {"CLAER"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            Dictionary<string, float> products = _availabilityProducts.ElementMerge();
            List<Commodity> goods = new List<Commodity>();

            foreach (var i in products)
            {
                goods.Add(new Commodity(i.Key, i.Value, DateTime.Today));
            }

            foreach (var i in goods)
            {
                _consumptionProducts.ChangeElement(i.Product, i.Weight);
            }

            Console.WriteLine("Добавить удаляемые продукты в список необходимых продуктов?");
            var cmd = Console.ReadLine();

            if (cmd == "Y" || cmd == "y")
            {
                foreach (var i in goods)
                {
                    _necessaryProducts.Add(i);
                }
            }

            _availabilityProducts.Clear();
            _availabilityProducts.Date = DateTime.Today;
            _availableRecipes.ProductСhanges = true;
        }
    }
}