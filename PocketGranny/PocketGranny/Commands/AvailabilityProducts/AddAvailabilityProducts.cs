using System;
using ConsoleUI;

namespace PocketGranny.Commands.AvailabilityProducts
{
    public class AddAvailabilityProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        private ListConsumptionProducts _consumptionProducts;

        private AvailableRecipes _availableRecipes;

        public AddAvailabilityProducts(Application app, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts, 
            ListConsumptionProducts consumptionProducts, AvailableRecipes availableRecipes)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
            _consumptionProducts = consumptionProducts;
            _availableRecipes = availableRecipes;
        }

        public string Name => "add";

        public string Help => "Добавить элемент в список доступных продуктов";

        public string Description => "Параметры: Имя, Вес, Дата покупки[День, Месяц, Год]";

        public string[] Synonyms => new[] { "ADD" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length < 2)
            {
                Console.WriteLine("Количество параметров должно быть не менее двух");
                return;
            }

            int indexWeight = parameters.Length - 1;
            DateTime dateOfPurchase;

            try
            {
                if (DateAvailability(parameters[parameters.Length - 1], out DateTime date))
                {
                    dateOfPurchase = date;
                    indexWeight--;
                }
                else
                {
                    dateOfPurchase = date;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(Description);
                Console.WriteLine("Добавить продукт с текущей датой(Y/)");

                string cmd = Console.ReadLine();

                if (cmd != "Y" && cmd != "y")
                {
                    return;
                }

                dateOfPurchase = DateTime.Today;
            }

            parameters[indexWeight] = parameters[indexWeight].Replace(".", ",");

            string[] name = new string[indexWeight];

            Array.Copy(parameters, name, indexWeight);

            string nameProduct = string.Join(" ", name);

            if (float.TryParse(parameters[indexWeight], out float a))
            {
                try
                {
                    var product = new Commodity(nameProduct, a, dateOfPurchase);

                    _availabilityProducts.Add(product);
                    
                    var id = _necessaryProducts.IndexOf(product);

                    if (id[2] != -1)
                    {
                        Console.WriteLine($"Удалить [{ product.ToString() }] из списка необходимых продуктов(Y/)");
                        var cmd = Console.ReadLine();

                        if (cmd == "Y" || cmd == "y")
                        {
                            _necessaryProducts.RemoveAt(id);
                        }                       
                    }

                    //_consumptionProducts.UpdateAmountDays(_necessaryProducts.Date);
                    //_necessaryProducts.Date = DateTime.Today;

                    _availabilityProducts.Date = DateTime.Today;
                    _consumptionProducts.Add(product.Product, DateTime.Today);
                    _availableRecipes.ProductСhanges = true;
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

        private bool DateAvailability(string line, out DateTime date)
        {
            string[] arg = line.Split(new char[] { '.' });
            
            if (arg.Length == 3)
            {
                if (int.TryParse(arg[0], out int day) && int.TryParse(arg[1], out int month) && int.TryParse(arg[2], out int year))
                {
                    try
                    {
                        date = new DateTime(day, month, year);

                        return true;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new ArgumentException("Дата продукта введена некорректно");
                    }
                }               
            }
            else
            {
                throw new ArgumentException("Дата продукта введена некорректно");
            }

            date = DateTime.Today;

            return false;
        }
    }
}