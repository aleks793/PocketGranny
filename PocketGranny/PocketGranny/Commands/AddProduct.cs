using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsoleUI;

namespace PocketGranny.Commands
{
    public class AddProduct : ICommand
    {
        private Application _app;

        public AddProduct(Application app)
        {
            _app = app;
        }

        public string Name => "add-product";

        public string Help => "Добавить продукт в базу";

        public string Description => "";

        public string[] Synonyms => new[] { "ADD-PRODUCT" };

        public void Execute(params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                Console.WriteLine("Команда не принимает параметров");
                return;
            }

            Console.WriteLine("Введите имя продукта:");
            Console.Write("> ");
            string nameProduct = Console.ReadLine();

            if (IsThereProduct(nameProduct))
            {
                Console.WriteLine("Продукт уже есть в базе");
                return;
            }

            Console.WriteLine("Введите категорию продукта:");
            Console.Write("> ");
            string categoryName = Console.ReadLine();

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

            var product = new Product(categoryName, nameProduct, expiryIn, weightUnit);

            SerializeProduct(product);

            Console.WriteLine("Продукт добавлен в базу");
        }

        private bool IsThereProduct(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataProduct.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Product[@name='{ name }']");

            if (xmlNode != null)
            {
                return true;
            }

            return false;
        }

        private void SerializeProduct(Product product)
        {
            var formatter = new XmlSerializer(typeof(Product));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    formatter.Serialize(xmlWriter, product);

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(stringWriter.ToString());

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(@"..\..\DataProduct.xml");
                    XmlElement xmlRoot = xmlDoc.DocumentElement;
                    xmlRoot.AppendChild(xmlDoc.ImportNode(xDoc.DocumentElement, true));
                    xmlDoc.Save(@"..\..\DataProduct.xml");
                }
            }
        }
    }
}