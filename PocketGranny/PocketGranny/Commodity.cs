using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class Commodity
    {
        private Product _product;

        private float _weight;

        private float _initialWeight;

        private DateTime _expiryDate;

        public Product Product
        {
            get => _product;
            set => _product = value;
        }

        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public float InitialWeight
        {
            get => _initialWeight;
            set => _initialWeight = value;
        }

        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set => _expiryDate = value;
        }

        public Commodity()
        {

        }

        public Commodity(string name, float weight, DateTime date)
        {
            try
            {
                Product = GetProduct(name);
            }
            catch (ArgumentException)
            {
                throw;
            }

            Weight = weight;
            InitialWeight = weight;
            ExpiryDate = date.AddDays(Product.ExpiryIn.TotalDays);
        }

        public Commodity(Product product, float weight, DateTime date)
        {
            Product = product;
            Weight = weight;
            InitialWeight = weight;
            ExpiryDate = date.AddDays(Product.ExpiryIn.TotalDays);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Commodity product && product.Product.Name == Product.Name && product.InitialWeight == InitialWeight)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (Product.WeightUnit.Length > 1 && Weight >= 1000)
            {
                return GetString((Weight / 1000).ToString(), (InitialWeight / 1000).ToString(), Product.WeightUnit[1]);
            }

            return GetString(Weight.ToString(), InitialWeight.ToString(), Product.WeightUnit[0]);
        }

        private string GetString(string weight, string initialWeight, string weightUnit)
        {
            if (InitialWeight - Weight > 0)
            {
                return $"{ Product.Name } ({ weight }/{ initialWeight } { weightUnit })";
            }

            return $"{ Product.Name } ({ weight } { weightUnit })";
        }

        public ConsoleColor GetColorProduct()
        {
            TimeSpan days = ExpiryDate.Subtract(DateTime.Today);

            if (days.Days <= 1)
            {
                return ConsoleColor.Red;
            }

            if (days.Days <= 4)
            {
                return ConsoleColor.DarkYellow;
            }

            return ConsoleColor.Green;
        }

        private Product GetProduct(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataProduct.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"Product[@name='{ name }']");

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