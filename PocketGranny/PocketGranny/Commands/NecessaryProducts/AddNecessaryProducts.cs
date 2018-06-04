using System;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class AddNecessaryProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public AddNecessaryProducts(Application app, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "add";

        public string Help => "Добавить элемент в список необходимых продуктов";

        public string Description => "Параметры: имя, вес";

        public string[] Synonyms => new[] {"ADD"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length < 2)
            {
                Console.WriteLine("Количество параметров должно быть не менее двух");
                return;
            }

            int indexWeight = parameters.Length - 1;

            parameters[indexWeight] = parameters[indexWeight].Replace(".", ",");

            string[] name = new string[indexWeight];

            Array.Copy(parameters, name, indexWeight);

            string nameProduct = string.Join(" ", name);

            if (float.TryParse(parameters[indexWeight], out float weight))
            {
                try
                {
                    var product = new Commodity(nameProduct, weight, DateTime.Today);

                    _necessaryProducts.Add(product);

                    var id = _availabilityProducts.IndexOf(product);

                    if (id[2] != -1)
                    {
                        Console.WriteLine($"Удалить [{ product.ToString() }] из списка доступных продуктов(Y/)");
                        var cmd = Console.ReadLine();

                        if (cmd == "Y" || cmd == "y")
                        {
                            _availabilityProducts.RemoveAt(id);
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Чтобы добавить продукт в базу воспользуйтесь командой [add-product]");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"Вес продукта [{parameters[indexWeight]}] введен некорректно");
                return;
            }
        }
    }
}