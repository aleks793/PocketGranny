using System;
using ConsoleUI;
using PocketGranny.Commands.PossibleRecipes;

namespace PocketGranny.Commands.Recipes
{
    public class DisplayRecipes : ICommand
    {
        private ListCategoriesRecipes _listCategoriesRecipes;

        private AvailableRecipes _availableRecipes;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public DisplayRecipes(ListCategoriesRecipes listCategoriesRecipes, AvailableRecipes availableRecipes, 
            ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _listCategoriesRecipes = listCategoriesRecipes;
            _availableRecipes = availableRecipes;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "display";

        public string Help => "Отображает список желаемых рецептов";

        public string Description => "";

        public string[] Synonyms => new[] { "DISPLAY" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметры");
                return;
            }

            if (_listCategoriesRecipes.Categories.Count == 0)
            {
                Console.WriteLine("Список рецептов пуст");
            }
            else
            {
                Console.WriteLine("Список для рецептов:");
                _listCategoriesRecipes.Print();
            }
            
            if (_availableRecipes.RecommendedRecipes.Count == 0)
            {
                return;
            }

            Console.WriteLine("Список рецептов, для которых есть часть продуктов");

            _availableRecipes.FindRecommendations(_availabilityProducts.GetProductsAll());

            var appRecipes = new Application();
            appRecipes.AddCommand(new ExitCommand(appRecipes));
            appRecipes.AddCommand(new ExplainCommand(appRecipes));
            appRecipes.AddCommand(new HelpCommand(appRecipes));
            appRecipes.AddCommand(new AddPossibleRecipes(_availableRecipes.RecommendedRecipes, _listCategoriesRecipes));
            appRecipes.AddCommand(new InfoPossibleRecipes(_availableRecipes.RecommendedRecipes, _availabilityProducts, _necessaryProducts));
            appRecipes.AddCommand(new DisplayPossibleRecipes(_availableRecipes.RecommendedRecipes));

            appRecipes.FindCommand("display").Execute();

            appRecipes.Run(Console.In);
        }
    }
}