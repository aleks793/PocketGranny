using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class RemoveNecessaryProducts : ICommand
    {
        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public RemoveNecessaryProducts(ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "remove";

        public string Help => "Удалить по индексу элемент из списка необходимых продуктов?";

        public string Description => "Параметры: индексы элементов";

        public string[] Synonyms => new[] {"REMOVE"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Количество параметров должно быть больше нуля");
                return;
            }

            var args = new List<string>();

            foreach (var i in parameters)
            {
                if (args.IndexOf(i) == -1)
                {
                    args.Add(i);
                }
            }

            args.Sort(CompareDinosByLength);
            args.Reverse();

            List<int[]> identifiers = new List<int[]>();

            foreach (var i in args.Distinct())
            {
                var levelsString = i.Split(':');
                var levels = new int[levelsString.Length];

                try
                {
                    for (var k = 0; k < levelsString.Length; k++)
                    {
                        if (!int.TryParse(levelsString[k], out levels[k]))
                        {
                            throw new ArgumentException($"Формат идентификатора { i } не верен");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Он будет пропущен");
                    continue;
                }

                identifiers.Add(levels);
            }

            foreach (var i in identifiers)
            {
                try
                {
                    var product = _necessaryProducts.FindElement(i);
                    Console.WriteLine($"Добавить [Name: { product.ToString() }] в список доступных продуктов");
                    var cmd = Console.ReadLine();

                    if (cmd == "Y" || cmd == "y")
                    {
                        _availabilityProducts.Add(product);
                    }

                    _necessaryProducts.RemoveAt(i);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine($"Первое значение [{ i[0] }] в идентификаторе находится за пределами допустимого диаппазона");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Он будет пропущен");
                    continue;
                }
            }
        }

        private static int CompareDinosByLength(string x, string y)
        {
            int retval = x.Length.CompareTo(y.Length);

            if (retval != 0)
            {
                return retval;
            }
            else
            {
                return x.CompareTo(y);
            }
        }
    }
}