using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.AvailabilityProducts
{
    public class CommandsAvailabilityProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        private ListConsumptionProducts _consumptionProducts;

        private AvailableRecipes _availableRecipes;

        public CommandsAvailabilityProducts(Application app, ListCategoriesCommodity availabilityProducts, ListConsumptionProducts consumptionProducts, AvailableRecipes availableRecipes)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
            _consumptionProducts = consumptionProducts;
            _availableRecipes = availableRecipes;
        }

        public string Name => "ap";

        public string Help => "Открыть список доступных продуктов";

        public string Description => "";

        public string[] Synonyms => new[] { "AP" };

        public void Execute(params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Открыт список доступных продуктов");
            Console.ResetColor();
            Console.WriteLine(Line);

            ChangesOverTime();

            _app.FindCommand("display").Execute();

            _app.Run(Console.In);

            _app.KeepRunning = true;
        }

        private void ChangesOverTime()
        {
            if (_availabilityProducts.RemovingSpoiledProducts())
            {
                Console.WriteLine("Испорченные продукты были удалены");
            }

            List<ConsumedCommodity> products = _consumptionProducts.RecommendedProducts();

            if (products.Count == 0)
            {
                return;
            }

            List<Commodity> goods = new List<Commodity>();

            foreach (var i in products)
            {
                goods.Add(new Commodity(i.Product, i.Weight, DateTime.Today));
            }

            var recommendations = new ListCategoriesCommodity(_availabilityProducts.Date);

            foreach (var i in _availabilityProducts.GetCommodityAll())
            {
                recommendations.Add(new Commodity(i.Product, i.Weight, i.ExpiryDate));
            }

            if (!recommendations.ChangeOverTime(goods))
            {
                return;
            }

            Console.WriteLine("Возможные изменения доступных продуктов:");

            recommendations.Print(true);

            Console.WriteLine($"Сохранить изменения?(Y/)");
            string cmd = Console.ReadLine();

            if (cmd == "Y" || cmd == "y")
            {
                _availabilityProducts = recommendations;
            }

            _availableRecipes.ProductСhanges = true;
        }

        private const string Line = "================================================";
    }
}