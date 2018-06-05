using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsoleUI;

namespace PocketGranny.Commands
{
    public class AddRecipe : ICommand
    {
        public string Name => "add-recipe";

        public string Help => "Добавить рецепт в базу";

        public string Description => "";

        public string[] Synonyms => new[] { "ADD-RECIPE" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметров");
                return;
            }

            Console.WriteLine("Введите имя рецепта:");
            Console.Write("> ");
            string nameRecipe = Console.ReadLine();

            if (IsThereRecipe(nameRecipe))
            {
                Console.WriteLine("Рецепт уже есть в базе");
                return;
            }

            Console.WriteLine("Введите категорию рецепта:");
            Console.Write("> ");
            string categoryName = Console.ReadLine();

            Console.WriteLine("Введите информацию о рецепте:");
            Console.Write("> ");
            string info = Console.ReadLine();

            Console.WriteLine("Введите продукты с весом использующиеся в рецепте через пробел(Пример: milk-500):");
            Console.Write("> ");
            string productsLine = Console.ReadLine();
            string[] productsString = productsLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (productsString.Length == 0)
            {
                Console.WriteLine("Продукты не были введены");
                return;
            }

            XmlDictionary<Product, float> products = new XmlDictionary<Product, float>();
            foreach (var i in productsString)
            {
                string[] productString = i.Split(new char[] { '-' });

                if (productString.Length != 2)
                {
                    Console.WriteLine($"Продукт [{ i }] введен некорректно");
                    return;
                }

                try
                {
                    var product = GetProduct(productString[0]);

                    if (!float.TryParse(productString[1], out float weightProduct))
                    {
                        Console.WriteLine($"Вес рецепта [{ productString[1] }] введен некорректно");
                        return;
                    }

                    products.Add(product, weightProduct);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            Console.WriteLine("Введите вес рецепта:");
            Console.Write("> ");
            string weightString = Console.ReadLine();
            if (!float.TryParse(weightString, out float weight))
            {
                Console.WriteLine($"Вес рецепта [{ weightString }] введен некорректно");
                return;
            }

            Console.WriteLine("Введите срок годности(дни):");
            Console.Write("> ");
            string day = Console.ReadLine();
            if (!int.TryParse(day, out int days))
            {
                Console.WriteLine($"Срок годности(дни) [{ day }] введен некорректно");
                return;
            }
            TimeSpan expiryIn = new TimeSpan(days, 0, 0, 0);

            Console.WriteLine("Введите меру измерения(возможно несколько параметров(Пример: g. kg.)):");
            Console.Write("> ");
            string measurement = Console.ReadLine();
            string[] weightUnit = measurement.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            var recipe = new Recipe(categoryName, nameRecipe, info, weight, expiryIn, products, weightUnit);

            SerializeRecipe(recipe);

            Console.WriteLine("Рецепт добавлен в базу");
        }

        private bool IsThereRecipe(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataRecipe.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Recipe[@name='{name}']");

            if (xmlNode != null)
            {
                return true;
            }

            return false;
        }

        private void SerializeRecipe(Recipe recipe)
        {
            var formatter = new XmlSerializer(typeof(Recipe));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    formatter.Serialize(xmlWriter, recipe);

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(stringWriter.ToString());

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(@"..\..\DataRecipe.xml");
                    XmlElement xmlRoot = xmlDoc.DocumentElement;
                    xmlRoot.AppendChild(xmlDoc.ImportNode(xDoc.DocumentElement, true));
                    xmlDoc.Save(@"..\..\DataRecipe.xml");
                }
            }
        }

        private Product GetProduct(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataProduct.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Product[@name='{name}']");

            if (xmlNode == null)
            {
                throw new ArgumentException($"Продукта с именем [{ name }] нет в базе");
            }

            XmlSerializer formatter = new XmlSerializer(typeof(Product));
            Product product = new Product();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.AppendChild(xDoc.ImportNode(xmlNode, true));
                xDoc.Save(memoryStream);
                memoryStream.Position = 0;
                product = (Product)formatter.Deserialize(XmlReader.Create(memoryStream));
            }

            return product;
        }
    }
}