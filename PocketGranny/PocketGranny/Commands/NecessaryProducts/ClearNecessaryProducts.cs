using System;
using System.Collections.Generic;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class ClearNecessaryProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public ClearNecessaryProducts(Application app, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "clear";

        public string Help => "Очистить список необходимых продуктов";

        public string Description => "";

        public string[] Synonyms => new[] {"CLAER"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            Console.WriteLine("Добавить удаляемые продукты в список доступных продуктов?");
            var cmd = Console.ReadLine();

            if (cmd == "Y" || cmd == "y")
            {
                Dictionary<string, float> products = _necessaryProducts.ElementMerge();

                foreach (var i in products)
                {
                    _availabilityProducts.Add(new Commodity(i.Key, i.Value, DateTime.Today));
                }
            }

            _necessaryProducts.Clear();
        }
    }
}