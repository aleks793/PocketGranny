using System;
using System.Collections.Generic;
using System.IO;
using ConsoleUI;

namespace PocketGranny.Commands
{
    public class AddCheck : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        private ListConsumptionProducts _consumptionProducts;

        private DateTime _date = DateTime.Today;

        public AddCheck(Application app, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts, ListConsumptionProducts consumptionProducts)
        {
            _app = app;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
            _consumptionProducts = consumptionProducts;
        }

        public string Name => "add";

        public string Help => "Считывает продукты из файла(чек)";

        public string Description => "";

        public string[] Synonyms => new[] {"ADD"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            List<Commodity> products = ReadingFile(@"..\..\Text.txt");

            foreach (var i in products)
            {
                var k = _necessaryProducts.IndexOf(i);

                if (k[2] != -1)
                {
                    _necessaryProducts.RemoveAt(k);
                }
            }

            _availabilityProducts.AddRange(products);

            if (_necessaryProducts.Date != null)
            {
                _consumptionProducts.UpdateAmountDays(_date, _necessaryProducts.Date);
            }

            _necessaryProducts.Date = _date;

            foreach (var i in products)
            {
                _consumptionProducts.Add(i.Product, _date);
            }
        }

        public List<Commodity> ReadingFile(string path)
        {
            StreamReader read = new StreamReader(path);
            string line = "";
            List<Commodity> products = new List<Commodity>();

            while (line != null)
            {
                line = read.ReadLine();

                if (line != null)
                {
                    string[] args = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (args.Length < 2)
                    {
                        Console.WriteLine($"В строке [ {line} ] недостаточно аргументов");
                        continue;
                    }

                    string[] name = new string[args.Length - 1];

                    Array.Copy(args, name, args.Length - 1);

                    string nameProduct = string.Join(" ", name);

                    if (nameProduct == "Date:")
                    {
                        if (DateTime.TryParse(args[args.Length - 1], out DateTime date))
                        {
                            _date = date;
                        }

                        continue;
                    }

                    args[args.Length - 1] = args[args.Length - 1].Replace(".", ",");

                    if (float.TryParse(args[args.Length - 1], out float a))
                    {
                        try
                        {
                            products.Add(new Commodity(nameProduct, a, _date));
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Чтобы добавить продукт в базу воспользуйтесь командой [add-product]");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Вес продукта [{args[args.Length - 1]}] введен некорректно");
                    }
                }
            }

            return products;
        }
    }
}