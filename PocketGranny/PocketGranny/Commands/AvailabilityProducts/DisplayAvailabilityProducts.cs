using System;
using ConsoleUI;

namespace PocketGranny.Commands.AvailabilityProducts
{
    public class DisplayAvailabilityProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        public DisplayAvailabilityProducts(Application app, ListCategoriesCommodity availabilityProducts)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
        }

        public string Name => "display";

        public string Help => "Отображает список доступных продуктов";

        public string Description => "";

        public string[] Synonyms => new[] {"DISPLAY"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            if (_availabilityProducts.Categories.Count == 0)
            {
                Console.WriteLine("Список доступных продуктов пуст");
                return;
            }

            _availabilityProducts.Print(true);
        }
    }
}