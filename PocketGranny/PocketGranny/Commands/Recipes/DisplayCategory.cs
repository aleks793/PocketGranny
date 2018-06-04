using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsoleUI;
using PocketGranny.Commands.PossibleRecipes;

namespace PocketGranny.Commands.Recipes
{
    public class DisplayCategory : ICommand
    {
        private Application _app;

        private ListCategoriesRecipes _listCategoriesRecipes;

        private ListCategoriesCommodity _availabilityProducts;

        private ListCategoriesCommodity _necessaryProducts;

        public DisplayCategory(Application app, ListCategoriesRecipes listCategoriesRecipes, ListCategoriesCommodity availabilityProducts, ListCategoriesCommodity necessaryProducts)
        {
            _app = app;
            _listCategoriesRecipes = listCategoriesRecipes;
            _availabilityProducts = availabilityProducts;
            _necessaryProducts = necessaryProducts;
        }

        public string Name => "display-category";

        public string Help => "Вывести все рецепты для категории";

        public string Description => "Параметры: категория";

        public string[] Synonyms => new[] { "DISPLAY-CATEGORY" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                Console.WriteLine("Количество параметров должно быть равно одному");
                return;
            }

            try
            {
                List<Recipe> recipes = GetRecipesFromCategory(parameters[0]);

                var appRecipes = new Application();
                appRecipes.AddCommand(new ExitCommand(appRecipes));
                appRecipes.AddCommand(new ExplainCommand(appRecipes));
                appRecipes.AddCommand(new HelpCommand(appRecipes));
                appRecipes.AddCommand(new AddPossibleRecipes(appRecipes, recipes, _listCategoriesRecipes));
                appRecipes.AddCommand(new InfoPossibleRecipes(appRecipes, recipes, _availabilityProducts, _necessaryProducts));
                appRecipes.AddCommand(new DisplayPossibleRecipes(appRecipes, recipes));

                appRecipes.FindCommand("display").Execute();

                appRecipes.Run(Console.In);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<Recipe> GetRecipesFromCategory(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataRecipe.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;

            var listNodes = xmlRoot.SelectNodes($"Recipe[@categoryName='{ name }']");

            if (listNodes.Count == 0)
            {
                throw new ArgumentException($"Рецептов с категорией [{ name }] нет в базе");
            }

            XmlSerializer formatter = new XmlSerializer(typeof(Recipe));
            List<Recipe> recipes = new List<Recipe>();

            for (int k = 0; k < listNodes.Count; k++)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlDocument xDoc = new XmlDocument();

                    xDoc.AppendChild(xDoc.ImportNode(listNodes[k], true));

                    xDoc.Save(memoryStream);
                    memoryStream.Position = 0;
                    recipes.Add((Recipe)formatter.Deserialize(XmlReader.Create(memoryStream)));
                }
            }

            return recipes;
        }
    }
}