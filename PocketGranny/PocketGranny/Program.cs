using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsoleUI;
using PocketGranny.Commands;
using PocketGranny.Commands.AvailabilityProducts;
using PocketGranny.Commands.NecessaryProducts;
using PocketGranny.Commands.Recipes;

namespace PocketGranny
{
    public class Program
    {
        private static Application _app = new Application();
        private static Application _appAvailabilityProducts = new Application();
        private static Application _appNecessaryProducts = new Application();
        private static Application _appRecipes = new Application();

        private static ListCategoriesCommodity _availabilityProducts = new ListCategoriesCommodity();
        private static ListCategoriesCommodity _necessaryProducts = new ListCategoriesCommodity();
        private static ListConsumptionProducts _consumptionProducts = new ListConsumptionProducts(2);
        private static ListCategoriesRecipes _listRecipes = new ListCategoriesRecipes();
        private static AvailableRecipes _availableRecipes = new AvailableRecipes();

        private const string Line = "...............................................";

        public static void Main(string[] args)
        {
            SetApp();
            SetAppAvailabilityProducts();
            SetAppNecessaryProducts();
            SetAppRecipes();
            Load();

            Console.WriteLine(Line);

            _app.FindCommand("ap").Execute();

            _app.Run(Console.In);

            Save();
        }

        private static void Load()
        {
            if (File.Exists("ap.xml") && File.Exists("np.xml") && File.Exists("cp.xml") && File.Exists("lr.xml"))
            {
                _availabilityProducts.Load("ap.xml");
                _necessaryProducts.Load("np.xml");
                _consumptionProducts.Load("cp.xml");
                _listRecipes.Load("lr.xml");
            }
            else
            {
                _consumptionProducts.AmountDays = GapBetweenPurchases();
            }
        }

        private static void Save()
        {
            _availabilityProducts.Save("ap.xml");
            _necessaryProducts.Save("np.xml");
            _consumptionProducts.Save("cp.xml");
            _listRecipes.Save("lr.xml");
        }

        private static TimeSpan GapBetweenPurchases()
        {
            Console.WriteLine("Сколько дней проходит, между вашими походами в магазин?");
            var amountDays = Console.ReadLine();

            if (int.TryParse(amountDays, out int a))
            {
                return new TimeSpan(a, 0, 0, 0);
            }
            else
            {
                Console.WriteLine("Раз вы не хотите мне помочь, я буду расчитывать рекомендации на 2 дня");

                return new TimeSpan(2, 0, 0, 0);
            }
        }

        private static void SetApp()
        {
            _app.AddCommand(new ExitCommand(_app));
            _app.AddCommand(new ExplainCommand(_app));
            _app.AddCommand(new HelpCommand(_app));
            _app.AddCommand(new AddCheck(_app, _availabilityProducts, _necessaryProducts, _consumptionProducts));
            _app.AddCommand(new AddProduct(_app));
            _app.AddCommand(new AddRecipe(_app));
            _app.AddCommand(new CommandsAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts, _consumptionProducts, _availableRecipes));
            _app.AddCommand(new CommandsNecessaryProducts(_appNecessaryProducts));
            _app.AddCommand(new CommandsRecipes(_appRecipes));
        }

        private static void SetAppAvailabilityProducts()
        {
            _appAvailabilityProducts.AddCommand(new ExitCommand(_appAvailabilityProducts));
            _appAvailabilityProducts.AddCommand(new ExplainCommand(_appAvailabilityProducts));
            _appAvailabilityProducts.AddCommand(new HelpCommand(_appAvailabilityProducts));
            _appAvailabilityProducts.AddCommand(new AddAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts, _necessaryProducts, _consumptionProducts, _availableRecipes));
            _appAvailabilityProducts.AddCommand(new DisplayAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts));
            _appAvailabilityProducts.AddCommand(new ChangeAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts, _consumptionProducts, _availableRecipes));
            _appAvailabilityProducts.AddCommand(new RemoveAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts, _necessaryProducts, _consumptionProducts, _availableRecipes));
            _appAvailabilityProducts.AddCommand(new ClearAvailabilityProducts(_appAvailabilityProducts, _availabilityProducts, _necessaryProducts, _consumptionProducts, _availableRecipes));
        }

        private static void SetAppNecessaryProducts()
        {
            _appNecessaryProducts.AddCommand(new ExitCommand(_appNecessaryProducts));
            _appNecessaryProducts.AddCommand(new ExplainCommand(_appNecessaryProducts));
            _appNecessaryProducts.AddCommand(new HelpCommand(_appNecessaryProducts));
            _appNecessaryProducts.AddCommand(new AddNecessaryProducts(_appNecessaryProducts, _availabilityProducts, _necessaryProducts));
            _appNecessaryProducts.AddCommand(new DisplayNecessaryProducts(_appNecessaryProducts, _availabilityProducts, _necessaryProducts, _consumptionProducts));
            _appNecessaryProducts.AddCommand(new ChangeNecessaryProducts(_appNecessaryProducts, _necessaryProducts));
            _appNecessaryProducts.AddCommand(new RemoveNecessaryProducts(_appNecessaryProducts, _availabilityProducts, _necessaryProducts));
            _appNecessaryProducts.AddCommand(new ClearNecessaryProducts(_appNecessaryProducts, _availabilityProducts, _necessaryProducts));
        }

        private static void SetAppRecipes()
        {
            _appRecipes.AddCommand(new ExitCommand(_appRecipes));
            _appRecipes.AddCommand(new ExplainCommand(_appRecipes));
            _appRecipes.AddCommand(new HelpCommand(_appRecipes));
            _appRecipes.AddCommand(new AddRecipes(_appRecipes, _listRecipes));
            _appRecipes.AddCommand(new ChangeRecipes(_appRecipes, _listRecipes));
            _appRecipes.AddCommand(new ClearRecipes(_appRecipes, _listRecipes));
            _appRecipes.AddCommand(new CookRecipes(_appRecipes, _listRecipes, _availabilityProducts, _necessaryProducts, _consumptionProducts));
            _appRecipes.AddCommand(new DisplayRecipes(_appRecipes, _listRecipes, _availableRecipes, _availabilityProducts, _necessaryProducts));
            _appRecipes.AddCommand(new DisplayCategory(_appRecipes, _listRecipes, _availabilityProducts, _necessaryProducts));
            _appRecipes.AddCommand(new InfoRecipes(_appRecipes, _listRecipes, _availabilityProducts, _necessaryProducts));
            _appRecipes.AddCommand(new RemoveRecipes(_appRecipes, _listRecipes));
        }
    }
}