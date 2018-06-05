using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsoleUI;

namespace PocketGranny.Commands.Recipes
{
    public class AddRecipes : ICommand
    {
        private ListCategoriesRecipes _listCategoriesRecipes;

        public AddRecipes(ListCategoriesRecipes listCategoriesRecipes)
        {
            _listCategoriesRecipes = listCategoriesRecipes;
        }

        public string Name => "add";

        public string Help => "Добавить элемент в список желаемых рецептов";

        public string Description => "Параметры: Имя, Вес";

        public string[] Synonyms => new[] { "ADD" };

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

            string nameRecipe = string.Join(" ", name);

            if (float.TryParse(parameters[indexWeight], out float weight))
            {
                try
                {
                    var recipe = GetRecipe(nameRecipe);

                    if (recipe.Weight != weight)
                    {
                        try
                        {
                            recipe.SetWeight(weight);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    try
                    {
                        _listCategoriesRecipes.Add(recipe);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine($"Вес рецепта [{ parameters[indexWeight] }] введен некорректно");
                return;
            }
        }

        private Recipe GetRecipe(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataRecipe.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Recipe[@name='{ name }']");

            if (xmlNode == null)
            {
                throw new ArgumentException($"Рецепта с именем [{ name }] нет в базе");
            }

            XmlSerializer formatter = new XmlSerializer(typeof(Recipe));
            Recipe recipe = new Recipe();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.AppendChild(xDoc.ImportNode(xmlNode, true));
                xDoc.Save(memoryStream);
                memoryStream.Position = 0;
                recipe = (Recipe)formatter.Deserialize(XmlReader.Create(memoryStream));
            }

            return recipe;
        }
    }
}