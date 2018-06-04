using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class AvailableRecipes
    {
        private bool _productСhanges = true;

        private List<Recipe> _recommendedRecipes = new List<Recipe>();

        public bool ProductСhanges
        {
            get => _productСhanges;
            set => _productСhanges = value;
        }

        public List<Recipe> RecommendedRecipes
        {
            get => _recommendedRecipes;
            set => _recommendedRecipes = value;
        }

        public void FindRecommendations(List<string> products)
        {
            if (!ProductСhanges)
            {
                return;
            }

            var recipes = GetRecommends(products);

            if (recipes.Count == 0)
            {
                return;
            }

            AddRecommends(recipes);
            ProductСhanges = false;
        }

        private void AddRecommends(List<string> recipes)
        {
            foreach (var i in recipes)
            {
                try
                {
                    RecommendedRecipes.Add(GetRecipe(i));
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }

        private List<string> GetRecommends(List<string> products)
        {
            List<string> recipes = new List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataRecipe.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;

            foreach (var i in products)
            {
                var listNodes = xmlRoot.SelectNodes($"Recipe/products/item/key/Product[@name='{ i }']");

                for (int k = 0; k < listNodes.Count; k++)
                {
                    string name = listNodes[k].ParentNode.ParentNode.ParentNode.ParentNode.Attributes["name"].Value;

                    if (!recipes.Contains(name))
                    {
                        recipes.Add(name);
                    }
                }
            }

            return recipes;
        }

        private Recipe GetRecipe(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataRecipe.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Product[@name='{ name }']");

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