using System;
using ConsoleUI;

namespace PocketGranny.Commands.NecessaryProducts
{
    public class ChangeNecessaryProducts : ICommand
    {
        private Application _app;

        private ListCategoriesCommodity _necessaryProducts;

        public ChangeNecessaryProducts(Application app, ListCategoriesCommodity necessaryProducts)
        {
            _app = app;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "change";

        public string Help => "Изменить по индексу вес элемента в списке необходимых продуктов";

        public string Description => "Параметры: индекс, вес";

        public string[] Synonyms => new[] {"CHANGE"};

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 2)
            {
                Console.WriteLine("Количество параметров должно быть равно двум");
                return;
            }

            parameters[1] = parameters[1].Replace(".", ",");

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

            if (float.TryParse(parameters[1], out float weight))
            {
                try
                {
                    _necessaryProducts.ChangeTo(levels, weight);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine($"Первое значение [{ levels[0] }] в идентификаторе находится за пределами допустимого диаппазона");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine($"Вес продукта [{ parameters[1] }] введен некорректно");
            }
        }
    }
}